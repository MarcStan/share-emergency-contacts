using ShareEmergencyContacts.Models;

namespace ShareEmergencyContacts.iOS
{
    public class IOSStorageProvider : IStorageProvider
    {
        public void WriteAllText(string filePath, string text)
        {
            throw new System.NotImplementedException();
        }

        public string ReadAllText(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}