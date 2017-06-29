using ShareEmergencyContacts.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidStorageProvider : IStorageProvider
    {
        public async Task WriteAllTextAsync(string filePath, string text)
        {
            await Task.Run(() => File.WriteAllText(filePath, text));
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await Task.Run(() => File.ReadAllText(filePath));
        }

        public async Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null)
        {
            if (!Directory.Exists(directory))
                return new List<string>();

            return await Task.Run(() =>
            {
                var files = Directory.GetFiles(directory, pattern);
                return files;
            });
        }

        public async Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            return await Task.Run(() => File.ReadAllLines(filePath));
        }

        public async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}