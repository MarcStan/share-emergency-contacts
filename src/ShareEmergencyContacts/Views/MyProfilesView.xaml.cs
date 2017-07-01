using ShareEmergencyContacts.ViewModels;
using System;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class MyProfilesView
    {
        public MyProfilesView()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            // create toolbar from code like a fucking pleb because xamarin xaml doesn't do shit (no Command is ever triggered at least on android/uwp)
            // but if we define it in code it magically works
            var dc = BindingContext as MyProfilesViewModel;
            if (dc != null && ToolbarItems.Count == 0)
            {
                ToolbarItems.Add(new ToolbarItem("Add", "add.png", () => dc.AddNewProfile()));
            }
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}