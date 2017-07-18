using System;

namespace ShareEmergencyContacts
{
    public interface IUnhandledExceptionHandler
    {
        event EventHandler<Exception> OnException;
    }
}