using ShareEmergencyContacts.ViewModels;
using System;

namespace ShareEmergencyContacts.Views
{
    public partial class ProfileVisualizerView
    {
        public ProfileVisualizerView()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
            CurrentPageChanged += OnCurrentPageChanged;
        }

        private void OnCurrentPageChanged(object sender, EventArgs eventArgs)
        {
            // TODO: will break once we localize
            switch (CurrentPage.Title)
            {
                case "Share":
                    ToolbarItems.Remove(Edit);
                    break;
                case "Details":
                    if (!ToolbarItems.Contains(Edit) && ((ProfileVisualizerViewModel)BindingContext).CanEdit)
                        ToolbarItems.Add(Edit);
                    break;
            }
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            var vm = BindingContext as ProfileVisualizerViewModel;
            if (vm != null)
            {
                // remove for contacts that can't share (received contacts)
                if (!vm.CanEdit)
                {
                    ToolbarItems.Remove(Edit);
                }
                if (vm.ShowBarcodeFirst)
                    CurrentPage = Children[1];
            }
        }
    }
}