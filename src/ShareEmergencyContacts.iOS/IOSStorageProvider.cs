using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ShareEmergencyContacts.Models;

namespace ShareEmergencyContacts.iOS
{
    public class IOSStorageProvider : IStorageProvider
    {
        public async Task WriteAllTextAsync(string filePath, string text)
        {
            await Task.Run(() =>
            {
                var fi = new FileInfo(filePath);
                if (!Directory.Exists(fi.DirectoryName))
                    Directory.CreateDirectory(fi.DirectoryName);
                File.WriteAllText(filePath, text);
            });
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
            await Task.Run(() =>
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            });
        }
    }
}