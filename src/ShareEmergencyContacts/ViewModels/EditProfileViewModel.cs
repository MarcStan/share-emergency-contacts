﻿using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly Action<EmergencyProfile> _onOk;
        private ProfileViewModel _selected;
        private readonly bool _creatingNew;

        public EditProfileViewModel(INavigationService navigationService, EmergencyProfile toEdit, Action<EmergencyProfile> onOk)
        {
            _creatingNew = toEdit == null;
            _navigationService = navigationService;
            _onOk = onOk;
            toEdit = toEdit ?? new EmergencyProfile();
            Selected = new ProfileViewModel(toEdit, null);
            SaveCommand = new Command(Ok);
            AddEmergencyContactCommand = new Command(AddEmergencyContact);
            AddInsuranceContactCommand = new Command(AddInsuranceContact);
        }

        public ICommand AddEmergencyContactCommand { get; }

        public ICommand AddInsuranceContactCommand { get; }

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

        public ICommand SaveCommand { get; }

        public override async void CanClose(Action<bool> callback)
        {
            // TODO: this is never called
            var dia = IoC.Get<IUserDialogs>();
            var r = await dia.ConfirmAsync($"Do you really want to cancel profile {(_creatingNew ? "creation" : "editing")}? All changes will be lost.", "Confirm abort", "Yes", "No");
            if (!r)
            {
                callback(false);
                return;
            }
            base.CanClose(callback);
        }

        public async void Ok()
        {
            _onOk(Selected.Actual);
            await _navigationService.GoBackAsync();

        }

        private void AddEmergencyContact()
        {
            throw new NotImplementedException();
        }

        private void AddInsuranceContact()
        {
            throw new NotImplementedException();
        }
    }
}