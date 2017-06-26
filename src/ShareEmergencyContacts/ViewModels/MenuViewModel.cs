using ShareEmergencyContacts.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly RootViewModel _fuck;
        public ICommand AboutCommand { get; set; }

        public MenuViewModel(RootViewModel fuck)
        {
            _fuck = fuck;
            AboutCommand = new Command(() => ShowAbout());
        }

        public void ShowMyProfiles()
        {


        }

        public void ShowSettings()
        {

        }

        public void ShowAbout()
        {
            var fuck = App.NavigationCancer;
            _fuck.MenuIsPresented = false;
            var ab = new AboutView();
            fuck.Navigation.PushAsync(ab);
        }
    }
}
