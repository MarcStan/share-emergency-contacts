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
    public class ProfileVisualizerViewModel : Screen
    {
        private ProfileViewModel _selected;
        private string _barcodeContent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="deleteAction"></param>
        /// <param name="editAction"></param>
        /// <param name="showBarcodeFirst">If true, shows the barcode fullscreen first.</param>
        public ProfileVisualizerViewModel(ProfileViewModel profile, Action<EmergencyProfile> deleteAction, Action<EmergencyProfile> editAction, bool showBarcodeFirst = false)
        {
            Selected = profile ?? throw new ArgumentNullException(nameof(profile));
            ShowBarcodeFirst = showBarcodeFirst;
            CanEdit = editAction != null;
            CanDelete = deleteAction != null;

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
                if (!CanEdit)
                    return;
                editAction(Selected.Actual);
            });
            DeleteCommand = new Command(() =>
            {
                if (!CanDelete)
                    return;
                deleteAction(Selected.Actual);
            });
            BarcodeContent = EmergencyProfile.ToRawText(Selected.Actual);
        }

        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

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

        public bool CanDelete { get; }

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