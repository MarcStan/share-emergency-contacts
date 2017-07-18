using Android.Runtime;
using System;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidUnhandledExceptionHandler : IUnhandledExceptionHandler
    {
        public AndroidUnhandledExceptionHandler()
        {
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                OnException?.Invoke(sender, args.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                OnException?.Invoke(sender, args.ExceptionObject as Exception);
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                OnException?.Invoke(sender, args.Exception);
            };
        }

        public event EventHandler<Exception> OnException;
    }
}