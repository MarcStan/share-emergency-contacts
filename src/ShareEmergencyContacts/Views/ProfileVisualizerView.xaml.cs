using ShareEmergencyContacts.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class ProfileVisualizerView
    {
        public ProfileVisualizerView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
            BindingContextChanged += OnBindingContextChanged;
            CurrentPageChanged += OnPageChanged;

            // setup barcode in background so view can load right away
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        // can't define in xaml as it throws exception if the barcode doesn't match (too big, wrong content, etc).
                        var barcode = new ZXingBarcodeImageView
                        {
                            BarcodeFormat = BarcodeFormat.QR_CODE,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        };
                        barcode.SetBinding(ZXingBarcodeImageView.BarcodeValueProperty,
                            new Binding("BarcodeContent", BindingMode.OneWay));
                        barcode.SetBinding(ZXingBarcodeImageView.BarcodeOptionsProperty,
                            new Binding("Options", BindingMode.OneWay));
                        Share.Children.Add(barcode);
                    }
                    catch (Exception)
                    {
                        // most likely due to barcode too big
                        Share.Children.Add(new Label
                        {
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            Text = "Failed to create QR code from profile. Probably too much data."
                        });
                    }
                });
            });
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

            vm?.PageChanged(CurrentPage.Title == "Share");
        }
    }
}