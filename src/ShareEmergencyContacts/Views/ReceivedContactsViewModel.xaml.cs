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
            if (e.SelectedItem != null)
                ((ListView)sender).SelectedItem = null;
        }
    }
}
