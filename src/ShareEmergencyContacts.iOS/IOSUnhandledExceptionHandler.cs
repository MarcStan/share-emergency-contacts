using System;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.iOS
{
    public class IOSUnhandledExceptionHandler : IUnhandledExceptionHandler
    {
        public IOSUnhandledExceptionHandler()
        {
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