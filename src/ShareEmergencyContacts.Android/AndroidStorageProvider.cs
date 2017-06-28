using ShareEmergencyContacts.Models;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidStorageProvider : IStorageProvider
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