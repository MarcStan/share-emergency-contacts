using ShareEmergencyContacts.ViewModels;
using System;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class ProfileVisualizerView
    {
        public ProfileVisualizerView()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            var vm = BindingContext as ProfileVisualizerViewModel;
            if (vm != null)
            {
                // remove for contacts that can't share or delete (received contacts)
                if (!vm.CanEdit)
                {
                    ToolbarItems.Remove(Edit);
                }
                if (!vm.CanDelete)
                {
                    ToolbarItems.Remove(Delete);
                }
                if (vm.ShowBarcodeFirst)
                    CurrentPage = Children[1];
            }
        }

        private void OnPageChanged(object sender, EventArgs e)
        {
            var vm = BindingContext as ProfileVisualizerViewModel;
            var page = (ContentPage)sender;

            vm?.PageChanged(page.Title == "Share");
        }
    }
}