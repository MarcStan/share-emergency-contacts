using ShareEmergencyContacts.ViewModels;
using System;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            // create toolbar from code like a fucking pleb because xamarin xaml doesn't do shit (no Command is ever triggered at least on android/uwp)
            // but if we define it in code it magically works
            var dc = BindingContext as MainViewModel;
            if (dc != null && ToolbarItems.Count == 0)
            {
                ToolbarItems.Add(new ToolbarItem("Scan", "qr.png", () => dc.ScanNewContact()));
                ToolbarItems.Add(new ToolbarItem("Share", "share.png", () => dc.ShareMyDetails()));

            }
        }
    }
}
