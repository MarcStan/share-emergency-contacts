using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class ReceivedContactsView
    {
        public ReceivedContactsView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                ((ListView)sender).SelectedItem = null;
        }
    }
}
