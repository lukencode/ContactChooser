using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;

namespace ContactChooser
{
    public partial class ContactChooserPage : PhoneApplicationPage
    {
        public event EventHandler<ContactChooserEventArgs> OnSelected = null;

        private static string _alphabet = "#abcdefghijklmnopqrstuvwxyz";
        private List<Group<ContactWithImage>> _emptyGroups;

        public ContactChooserPage()
        {
            InitializeComponent();

            initGroups();
        }

        private void initGroups()
        {
            _emptyGroups = (from c in _alphabet
                            select new Group<ContactWithImage>(c.ToString(), new List<ContactWithImage>())).ToList();
        }

        public void LoadContacts()
        {
            var contacts = new Contacts();
            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
            contacts.SearchAsync("", FilterKind.None, null);
        }

        private void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            var groupedContacts = from c in e.Results
                                  group c by convertToGroupHeader(c.DisplayName) into g
                                  select new Group<ContactWithImage>(g.Key.ToString(), g.Select(c => new ContactWithImage(c)).ToList());

            lstMain.ItemsSource = from t in groupedContacts.Union(_emptyGroups)
                                  orderby t.Title
                                  select t;
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
                
        private void lstMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = (lstMain.SelectedItem as ContactWithImage);

                if (OnSelected != null && item != null)
                {
                    OnSelected(this, new ContactChooserEventArgs() { Contact = item.Contact });

                    NavigationService.GoBack();
                }
            }
        }
    }
}