using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class ReceivedContactsView
    {
        public ReceivedContactsView()
        {
            InitializeComponent();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}
