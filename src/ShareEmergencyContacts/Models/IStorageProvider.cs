namespace ShareEmergencyContacts.Models
{
    public interface IStorageProvider
    {
        void WriteAllText(string filePath, string text);

        string ReadAllText(string filePath);
    }
}