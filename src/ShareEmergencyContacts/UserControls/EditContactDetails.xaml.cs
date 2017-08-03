using ShareEmergencyContacts.ViewModels.ForModels;
using System;

namespace ShareEmergencyContacts.UserControls
{
    public partial class EditContactDetails
    {
        public EditContactDetails()
        {
            InitializeComponent();
        }

        private void TextEntryCompleted(object sender, EventArgs e)
        {
            var vm = BindingContext as ContactViewModel;
            vm?.TextEntryCompleted();
        }
    }
}