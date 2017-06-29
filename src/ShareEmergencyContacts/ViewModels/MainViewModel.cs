using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ObservableCollection<EmergencyProfile> _existingContacts;
        private bool _isLoading;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ExistingContacts = new ObservableCollection<EmergencyProfile>();
            IsLoading = true;

            Task.Run(async () =>
            {
                var storage = IoC.Get<IStorageContainer>();
                var contacts = await storage.GetReceivedContactsAsync();
                Device.BeginInvokeOnMainThread(() =>
                {
                    ExistingContacts = new ObservableCollection<EmergencyProfile>(contacts);
                    IsLoading = false;
                });
            });
        }

        /// <summary>
        /// Gets whether the current view is still loading contacts from storage.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(nameof(IsLoading));
            }
        }

        public bool NoContacts => ExistingContacts.Count == 0;

        public ObservableCollection<EmergencyProfile> ExistingContacts
        {
            get => _existingContacts;
            set
            {
                _existingContacts = value;
                NotifyOfPropertyChange(nameof(ExistingContacts));
                NotifyOfPropertyChange(nameof(NoContacts));
            }
        }

        public void ScanNewContact()
        {
            _navigationService.NavigateToViewModelAsync<ScanCodeViewModel>();
        }

        public void ShareMyDetails()
        {

        }
    }
}