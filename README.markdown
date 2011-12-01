Windows Phone doesnt have a plain contact picker (not address or phone) so I made one!

```csharp
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
```