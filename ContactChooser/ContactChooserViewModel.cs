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
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Phone.UserData;
using System.Linq;
using System.Collections.ObjectModel;

namespace ContactChooser
{
    internal class ContactChooserViewModel : INotifyPropertyChanged
    {
        private PhoneApplicationPage _page;
        private ProgressIndicator _prog;

        private Action<List<Group<ContactWithImage>>> _onContactsLoaded;

        private ObservableCollection<Group<ContactWithImage>> _contacts;
        public ObservableCollection<Group<ContactWithImage>> Contacts 
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                NotifyPropertyChanged("Contacts");
            }
        }

        private static string _alphabet = "#abcdefghijklmnopqrstuvwxyz";
        private List<Group<ContactWithImage>> _emptyGroups;

        public ContactChooserViewModel(PhoneApplicationPage page)
        {
            _page = page;

            _emptyGroups = (from c in _alphabet
                            select new Group<ContactWithImage>(c.ToString(), new List<ContactWithImage>())).ToList();

            Contacts = new ObservableCollection<Group<ContactWithImage>>();
        }

        public void LoadContacts(Action<List<Group<ContactWithImage>>> onLoaded)
        {
            _onContactsLoaded = onLoaded;

            StartLoader("loading contacts");

            var contacts = new Contacts();
            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
            contacts.SearchAsync("", FilterKind.None, null);
        }

        private void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            var groupedContacts = from c in e.Results
                                  group c by convertToGroupHeader(c.DisplayName) into g
                                  select new Group<ContactWithImage>(g.Key.ToString(), g.Select(c => new ContactWithImage(c)).ToList());

            Contacts = (from t in groupedContacts.Union(_emptyGroups)
                        orderby t.Title
                        select t).ToObservableCollection();

            EndLoader();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _onContactsLoaded(Contacts.ToList());
                });
        }

        private char convertToGroupHeader(string displayName)
        {
            char l = displayName.ToLower()[0];
            if (l >= 'a' && l <= 'z')
            {
                return l;
            }
            return '#';
        }

        public void StartLoader(string text = "loading")
        {
            if (_page != null && _prog == null)
            {
                try
                {
                    _prog = _page.GetProgressIndicator();
                }
                catch { }
            };

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _prog.IsIndeterminate = true;
                _prog.IsVisible = true;
                _prog.Text = text;
            });
        }

        public void EndLoader()
        {
            if (_prog == null) return;

            _prog.IsIndeterminate = false;
            _prog.IsVisible = false;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                });
            }
        }
    }
}
