using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ObservableCollection<EmergencyProfile> _existingContacts;
        private bool _isLoading;

        public static EmergencyProfile ReceivedContact;

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

        public async Task OnPageActivateAsync()
        {
            if (ReceivedContact != null)
            {
                var newContact = ReceivedContact;
                ReceivedContact = null;
                // show overlay asking user for a new name; do not allow existing names
                var forbidden = _existingContacts.Select(c => c.ProfileName).ToArray();
                var name = await AskUserForNameAsync(forbidden, newContact.ProfileName);
                // if name is null, user doesn't want to save contact
                if (name != null)
                {
                    newContact.ProfileName = name;
                    await IoC.Get<IStorageContainer>().SaveReceivedContact(newContact);
                }
            }
        }

        /// <summary>
        /// Displays alert box and returns the user selected name that is not in forbidden list.
        /// Also prevents empty strings, etc.
        /// Returns null on user cancel.
        /// </summary>
        /// <param name="forbidden"></param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        private async Task<string> AskUserForNameAsync(string[] forbidden, string defaultName = null)
        {
            var prompt = new PromptConfig
            {
                Text = defaultName,
                Message = "Enter a name for the profile",
                CancelText = "Cancel",
                OkText = "Ok",
                Title = "Set profile name",
                Placeholder = "profile name",
                OnTextChanged = args =>
                {
                    args.IsValid = !string.IsNullOrWhiteSpace(args.Value) && !forbidden.Contains(args.Value);
                }
            };
            var dia = IoC.Get<IUserDialogs>();
            while (true)
            {
                var result = await dia.PromptAsync(prompt);
                if (result.Ok)
                {
                    var name = result.Text;
                    if (!forbidden.Contains(name))
                        return result.Value;

                    dia.Alert("The name is already used!", "Name in use", "Ok");
                }
                else
                {
                    var discardResult = await dia.ConfirmAsync("Do you really want to discard the contact? It won't be saved.", "Discard contact", "Yes", "No");
                    if (!discardResult)
                    {
                        // allow user to set name again
                        continue;
                    }
                    return null;
                }
            }
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