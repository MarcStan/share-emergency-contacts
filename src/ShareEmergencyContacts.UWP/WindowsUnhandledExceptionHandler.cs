using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsUnhandledExceptionHandler : IUnhandledExceptionHandler
    {
        public WindowsUnhandledExceptionHandler()
        {
            Application.Current.UnhandledException += (sender, args) =>
            {
                OnException?.Invoke(sender, args.Exception);
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                OnException?.Invoke(sender, args.Exception);
            };
        }

        public event EventHandler<Exception> OnException;
    }
}