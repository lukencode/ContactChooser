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
using Microsoft.Phone.UserData;
using System.Windows.Media.Imaging;

namespace ContactChooser
{
    public class ContactWithImage
    {
        public Contact Contact { get; private set; }
        public string DisplayName
        {
            get
            {
                return Contact.DisplayName;
            }
        }

        public BitmapImage Image
        {
            get
            {
                var bmp = new BitmapImage();
                var img = Contact.GetPicture();

                if (img == null)
                    return null;

                bmp.SetSource(img);
                return bmp;
            }
        }

        public ContactWithImage(Contact contact)
        {
            Contact = contact;
        }
    }
}
