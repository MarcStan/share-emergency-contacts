using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class MyProfilesView
    {
        public MyProfilesView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}