using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace ContactChooser
{
    public class ContactChooserTask
    {
        private static Uri _pageUri = new Uri("/ContactChooser;component/ContactChooserPage.xaml", UriKind.RelativeOrAbsolute);
        private PhoneApplicationFrame _frame;
        private object _frameContentWhenOpened;
        private NavigationInTransition _savedNavigationInTransition;
        private NavigationOutTransition _savedNavigationOutTransition;
        private ContactChooserPage _chooserPage;

        public event EventHandler<ContactChooserEventArgs> OnSelected = null;

        public ContactChooserTask()
        {

        }

        public void Show()
        {
            if (_frame == null)
            {
                // Hook up to necessary events and navigate
                _frame = Application.Current.RootVisual as PhoneApplicationFrame;
                _frameContentWhenOpened = _frame.Content;

                var frameContentWhenOpenedAsUiElement = _frameContentWhenOpened as UIElement;
                if (null != frameContentWhenOpenedAsUiElement)
                {
                    _savedNavigationInTransition = TransitionService.GetNavigationInTransition(frameContentWhenOpenedAsUiElement);
                    TransitionService.SetNavigationInTransition(frameContentWhenOpenedAsUiElement, null);
                    _savedNavigationOutTransition = TransitionService.GetNavigationOutTransition(frameContentWhenOpenedAsUiElement);
                    TransitionService.SetNavigationOutTransition(frameContentWhenOpenedAsUiElement, null);
                }

                _frame.Navigated += HandleFrameNavigated;

                if (_frame.GetType() == typeof(PhoneApplicationFrame))
                    _frame.NavigationStopped += HandleFrameNavigationStoppedOrFailed;

                _frame.NavigationFailed += HandleFrameNavigationStoppedOrFailed;

                _frame.Navigate(_pageUri);
            }
        }

        private void Close()
        {
            // Unhook from events
            if (_frame != null)
            {
                _frame.Navigated -= HandleFrameNavigated;
                _frame.NavigationStopped -= HandleFrameNavigationStoppedOrFailed;
                _frame.NavigationFailed -= HandleFrameNavigationStoppedOrFailed;

                // Restore host page transitions for the completed "popup" navigation
                var frameContentWhenOpenedAsUiElement = _frameContentWhenOpened as UIElement;
                if (null != frameContentWhenOpenedAsUiElement)
                {
                    TransitionService.SetNavigationInTransition(frameContentWhenOpenedAsUiElement, _savedNavigationInTransition);
                    _savedNavigationInTransition = null;
                    TransitionService.SetNavigationOutTransition(frameContentWhenOpenedAsUiElement, _savedNavigationOutTransition);
                    _savedNavigationOutTransition = null;
                }

                _frame = null;
                _frameContentWhenOpened = null;
                _chooserPage = null;
            }
        }

        private void HandleFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content == _frameContentWhenOpened)
            {
                // Navigation to original page; close the picker page
                Close();
            }
            else if(e.Content is ContactChooserPage)
            {
                _chooserPage = e.Content as ContactChooserPage;
                _chooserPage.OnSelected += OnSelected;

                _chooserPage.LoadContacts();
            }
        }

        private void HandleFrameNavigationStoppedOrFailed(object sender, EventArgs e)
        {
            // Abort
            Close();
        }
    }
}
