using Caliburn.Micro;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.QrCode;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Viewmodel for a single selected profile with all its emergency contacts and 
    /// </summary>
    public class SelectedProfileViewModel : ViewModelBase
    {
        private ProfileViewModel _selected;
        private bool _showBarcode;
        private string _barcodeContent;

        public SelectedProfileViewModel(ProfileViewModel profile, bool showBarcode = false, bool canShare = false)
        {
            Selected = profile ?? throw new ArgumentNullException(nameof(profile));
            ShowBarcode = showBarcode;
            CanShare = canShare;

            var appInfo = IoC.Get<IAppInfoProvider>();
            // margin of 10 around each side of the screen so make it roughly the size of the screen
            // however limit to reasonable pixel size because it's a fucking QR code. it can't have aliasing artifacts
            // because all lines are axis aligned
            // no point in having a 2048*2048 barcode on those ridiculous modern screens
            var s = Math.Min(1024, appInfo.ScreenWidth - 20);
            Options = new QrCodeEncodingOptions
            {
                Width = s,
                Height = s
            };
            EditCommand = new Command(() => { ShowBarcode = false; });
            ShareCommand = new Command(() => { ShowBarcode = true; });
        }

        public ICommand ShareCommand { get; }

        public ICommand EditCommand { get; }

        public string BarcodeContent
        {
            get => _barcodeContent;
            private set
            {
                if (value == _barcodeContent) return;
                _barcodeContent = value;
                NotifyOfPropertyChange(nameof(BarcodeContent));
            }
        }

        public bool ShowBarcode
        {
            get => _showBarcode;
            private set
            {
                if (value == _showBarcode) return;
                _showBarcode = value;
                // update barcode anytime user toggles to share view as he could have updated the values
                if (ShowBarcode)
                    BarcodeContent = EmergencyProfile.ToRawText(Selected.Actual);
                NotifyOfPropertyChange(nameof(ShowBarcode));
            }
        }

        public bool CanShare { get; }

        public QrCodeEncodingOptions Options { get; }

        public ProfileViewModel Selected
        {
            get => _selected;
            private set
            {
                if (Equals(value, _selected)) return;
                _selected = value;
                NotifyOfPropertyChange(nameof(Selected));
            }
        }
    }
}