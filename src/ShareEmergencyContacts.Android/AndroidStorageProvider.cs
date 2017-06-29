using System.Collections.Generic;
using System.Threading.Tasks;
using ShareEmergencyContacts.Models;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidStorageProvider : IStorageProvider
    {
        public Task WriteAllTextAsync(string filePath, string text)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ReadAllTextAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null)
        {
            return new List<string>();
        }

        public Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteFileAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}