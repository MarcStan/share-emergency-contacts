using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
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
    public class ProfileVisualizerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ProfileViewModel _selected;
        private string _barcodeContent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="profile"></param>
        /// <param name="showBarcodeFirst">If true, shows the barcode fullscreen first.</param>
        /// <param name="canEdit"></param>
        public ProfileVisualizerViewModel(INavigationService navigationService, ProfileViewModel profile, bool showBarcodeFirst = false, bool canEdit = false)
        {
            _navigationService = navigationService;
            Selected = profile ?? throw new ArgumentNullException(nameof(profile));
            ShowBarcodeFirst = showBarcodeFirst;
            CanEdit = canEdit;

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
            EditCommand = new Command(() =>
            {
                if (!canEdit)
                    return;
                _navigationService.NavigateToViewAsync<EditProfileViewModel>();
            });

            BarcodeContent = EmergencyProfile.ToRawText(Selected.Actual);
        }

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

        public bool ShowBarcodeFirst { get; }

        public bool CanEdit { get; }

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