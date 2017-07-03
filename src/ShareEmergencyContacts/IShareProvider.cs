namespace ShareEmergencyContacts
{
    public interface IShareProvider
    {
        void ShareUrl(string url, string title, string message);
    }
}