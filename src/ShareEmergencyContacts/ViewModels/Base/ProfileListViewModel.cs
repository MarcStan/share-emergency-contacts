using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
#if DEBUG
using System.Collections.Generic;
#endif

namespace ShareEmergencyContacts.ViewModels.Base
{
    /// <summary>
    /// Viewmodel that works with both contacts and own profiles.
    /// </summary>
    public abstract class ProfileListViewModel : Screen
    {
        private readonly bool _workWithMyProfiles;
        private BindableCollection<ProfileViewModel> _existingContacts;
        private bool _isLoading;
        private ProfileViewModel _selectedItem;
        private DateTime? _lastClick;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workWithMyProfiles">If true, works with my profiles otherwise with contacts.</param>
        protected ProfileListViewModel(bool workWithMyProfiles)
        {
            _workWithMyProfiles = workWithMyProfiles;
            ExistingContacts = new BindableCollection<ProfileViewModel>();
            IsLoading = true;

            Task.Run(async () =>
            {
                var storage = IoC.Get<IStorageContainer>();
                var contacts = _workWithMyProfiles
                    ? (await storage.GetProfilesAsync()).ToList()
                    : (await storage.GetReceivedContactsAsync()).ToList();
#if DEBUG
                if (contacts.Count == 0)
                {
                    // insert mock data
                    contacts = LoadMockContacts(workWithMyProfiles);
                    if (workWithMyProfiles)
                    {
                        foreach (var c in contacts)
                        {
                            // must not be set on own profiles
                            c.ExpirationDate = null;
                        }
                    }
                }
#endif
                var profiles = contacts.Select(Create).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    ExistingContacts = new BindableCollection<ProfileViewModel>(profiles);
                    IsLoading = false;
                });
            });
        }

        internal ProfileViewModel Create(EmergencyProfile c) => new ProfileViewModel(c, async p => await ConfirmDelete(p), async p => await ConfirmRename(p));

        public string CenterText
        {
            get
            {
                if (IsLoading)
                    return "Loading...";

                return _workWithMyProfiles
                    ? "No profiles set up yet. Press the plus icon to add one!"
                    : "No contacts have been received yet. Press the scan icon to scan one!";
            }
        }

        public ProfileViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null) return;
                _selectedItem = value;
                if (!_lastClick.HasValue || (DateTime.Now - _lastClick.Value).TotalSeconds > 1)
                {
                    // uwp still has double click bug, so rate limit this shit
                    // incidently this works out for android as well where user can actual press the item twice before the page loads (resulting in two pages being openend)
                    // with the rate limit we also prevent the double page open
                    _lastClick = DateTime.Now;
                    ProfileSelected(_selectedItem);
                }
                _selectedItem = null;
                NotifyOfPropertyChange(nameof(SelectedItem));
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
                NotifyOfPropertyChange(nameof(CenterText));
            }
        }

        public bool NoContacts => ExistingContacts == null || ExistingContacts.Count == 0;

        public BindableCollection<ProfileViewModel> ExistingContacts
        {
            get => _existingContacts;
            set
            {
                _existingContacts = value;
                NotifyOfPropertyChange(nameof(ExistingContacts));
                NotifyOfPropertyChange(nameof(NoContacts));
            }
        }

#if DEBUG
        private static List<EmergencyProfile> LoadMockContacts(bool myProfile)
        {
            var contacts = new List<EmergencyProfile>();
            var profile = new EmergencyProfile
            {
                ProfileName = "Marc",
                FirstName = "Marc",
                LastName = "Stan",
                Address = "Street 1" + Environment.NewLine +
                              "12345 LegitCity",
                BirthDate = new DateTime(1989, 1, 13),
                HeightInCm = 200,
                WeightInKg = 100,
                Note = "This is a note",
                PhoneNumbers = new List<PhoneNumber>
                    {
                        new PhoneNumber(PhoneType.Home, "555 12345"),
                        new PhoneNumber(PhoneType.Mobile, "+1 555 12345"),
                    },
                InsuranceContacts = new List<EmergencyContact>
                    {
                        new EmergencyContact
                        {
                            FirstName = "POLICE",
                            PhoneNumbers = new List<PhoneNumber>
                            {
                                new PhoneNumber(PhoneType.Work, "+49 110")
                            }
                        }
                    }
            };
            var names = new [] { "Eva Page", "Richard Dalton", "Jon Doe", "Jane Doe", "Peter Moors", "Kyle Spear", "Sylvia Franklin" };
            var days = new int[] { 1, 3, 4, 7, 7, 14, 28 };
            var heights = new int[] { 160, 180, 175, 165, 190, 180, 155 };
            var weights = new int[] { 160, 89, 80, 67, 90, 82, 55 };
            for(int i = 0; i < names.Length; i++)
            {
                if (!myProfile)
                {
                    var name = names[i].Split(' ');
                    var first = name[0];
                    var last = name[1];
                    profile.FirstName = profile.ProfileName = first;
                    profile.LastName = last;
                    profile.HeightInCm = heights[i];
                    profile.WeightInKg = weights[i];
                }
                profile.ExpirationDate = DateTime.Now.AddDays(days[i]);
                contacts.Add(profile.CloneFull());
            }
            return contacts;
        }
#endif

        /// <summary>
        /// Adds the specific profile to the current collection by validating that its name is unique.
        /// </summary>
        /// <param name="profile"></param>
        public async Task<bool> AddAsync(EmergencyProfile profile)
        {
            if (profile == null)
                return false;

            // show overlay asking user for a new name; do not allow existing names
            var forbidden = _existingContacts.Select(c => c.ProfileName).ToArray();
            var name = await AskUserForNameAsync(forbidden, profile.ProfileName, false);
            // if name is null, user doesn't want to save contact
            if (name != null)
            {
                profile.ProfileName = name;
                var storage = IoC.Get<IStorageContainer>();
                var dia = IoC.Get<IUserDialogs>();
                // insert at correct position, sorted by (file)name
                // otherwise the order will be different the next time the user starts the app
                var idx = GetInsertIndexFor(profile.ProfileName);
                ExistingContacts.Insert(idx, new ProfileViewModel(profile, async e => await ConfirmDelete(e), async p => await ConfirmRename(p)));
                NotifyOfPropertyChange(nameof(NoContacts));
                if (_workWithMyProfiles)
                {
                    await storage.SaveProfileAsync(profile);
                    dia.Toast("Added new profile!");
                }
                else
                {
                    await storage.SaveReceivedContactAsync(profile);
                    dia.Toast("Added new contact!");
                }
                await IoC.Get<INavigationService>().GoBackAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the correct index to use InsertAt on for the new name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetInsertIndexFor(string name)
        {
            for (int i = 0; i < ExistingContacts.Count; i++)
            {
                if (ExistingContacts[i].ProfileName.CompareTo(name) < 0)
                    continue;
                return i;
            }
            return ExistingContacts.Count;
        }

        /// <summary>
        /// Displays alert box and returns the user selected name that is not in forbidden list.
        /// Also prevents empty strings, etc.
        /// Returns null on user cancel.
        /// </summary>
        /// <param name="forbidden"></param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        private async Task<string> AskUserForNameAsync(string[] forbidden, string defaultName = null, bool discardAlert = true)
        {
            var dia = IoC.Get<IUserDialogs>();
            var name = defaultName ?? "";
            while (true)
            {
                var prompt = new PromptConfig
                {
                    Text = name,
                    Message = "Enter a name for the profile",
                    CancelText = "Cancel",
                    OkText = "Ok",
                    Title = "Set profile name",
                    Placeholder = "profile name",
                    OnAction = null,
                    OnTextChanged = args =>
                    {
                        args.IsValid = !string.IsNullOrWhiteSpace(args.Value) && !ContainsInvalidChars(args.Value);
                    }
                };
                var result = await dia.PromptAsync(prompt);
                if (result.Ok)
                {
                    name = result.Text?.Trim() ?? "";
                    if (ContainsInvalidChars(name))
                    {
                        await dia.AlertAsync("Profile names may only contain letters, digits, spaces or any of these: _-+()[]", "Invalid name", "Ok");
                        continue;
                    }
                    if (!forbidden.Contains(name))
                        return result.Value;

                    await dia.AlertAsync("The name is already used!", "Name in use", "Ok");
                }
                else
                {
                    if (!discardAlert)
                        return null;

                    var discardResult = await dia.ConfirmAsync("Do you really want to discard the contact? It won't be saved.", "Discard changes", "Yes", "No");
                    if (discardResult)
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the name contains any forbidden chars.
        /// All digits/letters are allowed as well as " _-+()[]"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool ContainsInvalidChars(string name)
        {
            var allowed = new[] { ' ', '_', '-', '+', '(', ')', '[', ']' };
            return name.ToCharArray().Any(c => !char.IsLetterOrDigit(c) && !allowed.Contains(c));
        }

        public async Task ConfirmRename(ProfileViewModel profile)
        {
            // show overlay asking user for a new name; do not allow existing names except for the name of the profile itself
            var forbidden = _existingContacts.Select(c => c.ProfileName).Except(new[] { profile.ProfileName }).ToArray();
            var name = await AskUserForNameAsync(forbidden, profile.ProfileName, false);
            if (name != null)
            {
                // set new name
                var old = profile.ProfileName;
                var oldIndex = GetInsertIndexFor(old);
                var newIndex = GetInsertIndexFor(name);
                profile.ProfileName = name;
                var storage = IoC.Get<IStorageContainer>();
                if (old != name)
                {
                    // delete old file as the name was changed
                    // just need mock object to derive filename from profile name
                    var mock = new EmergencyProfile { ProfileName = old };
                    if (_workWithMyProfiles)
                        await storage.DeleteProfileAsync(mock);
                    else
                        await storage.DeleteReceivedContactAsync(mock);
                }
                if (oldIndex != newIndex)
                {
                    // name different enough that we need to reorder list
                    ExistingContacts.Remove(profile);
                    // after removing it, recalc new index again (could be shifted by one if old name was sorted before new name)
                    newIndex = GetInsertIndexFor(name);
                    ExistingContacts.Insert(newIndex, profile);
                }
                else
                {
                    profile.Refresh();
                }
                var dia = IoC.Get<IUserDialogs>();
                if (_workWithMyProfiles)
                {
                    await storage.SaveProfileAsync(profile.Actual);
                    dia.Toast("Updated profile name!");
                }
                else
                {
                    await storage.SaveReceivedContactAsync(profile.Actual);
                    dia.Toast("Updated contact name!");
                }
            }
        }

        public async Task<bool> ConfirmDelete(ProfileViewModel profile)
        {
            var dia = IoC.Get<IUserDialogs>();
            string expiry;
            if (profile.Actual.ExpirationDate.HasValue)
            {
                var days = (profile.Actual.ExpirationDate.Value - DateTime.Now).Days + 1;
                expiry = $" It would expire in {days} day" + (days == 1 ? "" : "s") + ".";
            }
            else expiry = null;
            var type = _workWithMyProfiles ? "profile" : "received contact";
            var r = await dia.ConfirmAsync($"Really delete {type} '{profile.ProfileName}'?" + expiry, "Really delete?", "Yes", "No");
            if (!r)
                return false;

            var storage = IoC.Get<IStorageContainer>();
            if (_workWithMyProfiles)
            {
                await storage.DeleteProfileAsync(profile.Actual);
            }
            else
            {
                await storage.DeleteReceivedContactAsync(profile.Actual);
            }

            ExistingContacts.Remove(profile);
            NotifyOfPropertyChange(nameof(NoContacts));
            return true;
        }

        protected abstract void ProfileSelected(ProfileViewModel profile);
    }
}