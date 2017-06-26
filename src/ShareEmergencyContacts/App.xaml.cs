
using ShareEmergencyContacts.Views;
using Xamarin.Forms;

namespace ShareEmergencyContacts
{
    public partial class App
    {
        public static NavigationPage NavigationCancer;

        public App()
        {
            InitializeComponent();

            // xamarin is a steaming pile of shit
            var root = new RootView();
            var nav = NavigationCancer = new NavigationPage(new MainView
            {
                Title = "Main"
            });
            root.Detail = nav;
            MainPage = root;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
