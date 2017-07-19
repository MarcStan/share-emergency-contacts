using ShareEmergencyContacts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidIOSStorageProvider : IStorageProvider
    {
        public async Task WriteAllTextAsync(string filePath, string text)
        {
            await Task.Run(() =>
            {
                var path = GetFullPath(filePath, true);
                File.WriteAllText(path, text);
            });
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                filePath = GetFullPath(filePath);
                return File.ReadAllText(filePath);
            });
        }

        public async Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null)
        {
            return await Task.Run(() =>
            {
                directory = GetFullPath(directory);
                if (!Directory.Exists(directory))
                    return new string[0];
                var files = Directory.GetFiles(directory, pattern);
                return files;
            });
        }

        public async Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var path = GetFullPath(filePath);
                return File.ReadAllLines(path);
            });
        }

        public async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() =>
            {
                var path = GetFullPath(filePath);
                if (File.Exists(path))
                    File.Delete(path);
            });
        }

        private string GetFullPath(string relative, bool createIfMissing = false)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (!relative.StartsWith(documentsPath))
            {
                if (relative.StartsWith("/"))
                    relative = relative.Substring(1);
                relative = Path.Combine(documentsPath, relative);
            }
            var fi = new FileInfo(relative);
            if (createIfMissing)
            {
                if (!Directory.Exists(fi.DirectoryName))
                    Directory.CreateDirectory(fi.DirectoryName);
            }
            return relative;
        }
    }
}