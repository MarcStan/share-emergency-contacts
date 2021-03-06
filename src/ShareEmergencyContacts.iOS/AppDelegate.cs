using Caliburn.Micro;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace ShareEmergencyContacts.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {

        private readonly CaliburnAppDelegate _caliburn = new CaliburnAppDelegate();

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();

            // prevent iOS from auto  matching statusbar color to navbar color
            // (will happily change background color from white to black but keeps time/battery/etc icons always black -> invisible)
            // from https://forums.xamarin.com/discussion/17922/navigationpage-statusbar-color
            // also requires entries in plist

            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, false);
            // this has the nice addition of removing status bar on splashscreen
            // but now the app is running, so show it again
            UIApplication.SharedApplication.SetStatusBarHidden(false, false);

            LoadApplication(IoC.Get<App>());

            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            return base.FinishedLaunching(app, options);
        }
    }
}
