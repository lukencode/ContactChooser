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

namespace ContactChooser.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var contactChooser = new ContactChooserTask();
            contactChooser.OnSelected += new EventHandler<ContactChooserEventArgs>(contactChooser_OnSelected);
            contactChooser.Show();
        }

        void contactChooser_OnSelected(object sender, ContactChooserEventArgs e)
        {
            MessageBox.Show(e.Contact.DisplayName);
        }
    }
}