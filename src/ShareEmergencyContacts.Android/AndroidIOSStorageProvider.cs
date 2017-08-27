using ShareEmergencyContacts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidIOSStorageProvider : IStorageProvider
    {
        public Task WriteAllTextAsync(string filePath, string text)
        {
            return Task.Run(() =>
            {
                var path = GetFullPath(filePath, true);
                File.WriteAllText(path, text);
            });
        }

        public Task<string> ReadAllTextAsync(string filePath)
        {
            return Task.Run(() =>
            {
                filePath = GetFullPath(filePath);
                return File.ReadAllText(filePath);
            });
        }

        public Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null)
        {
            return Task.Run(() =>
            {
                directory = GetFullPath(directory);
                if (!Directory.Exists(directory))
                    return new string[0];
                var files = Directory.GetFiles(directory, pattern);
                return (IReadOnlyList<string>)files;
            });
        }

        public Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var path = GetFullPath(filePath);
                return (IReadOnlyList<string>)File.ReadAllLines(path);
            });
        }

        public Task DeleteFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var path = GetFullPath(filePath);
                if (File.Exists(path))
                    File.Delete(path);
            });
        }

        public Task SaveExternallyAsync(string filename, string content)
        {
            return Task.Run(() =>
            {
#if __ANDROID__
                var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                Directory.CreateDirectory(dir.AbsolutePath);
                File.WriteAllText(Path.Combine(dir.AbsolutePath, filename), content);
#else
                throw new NotSupportedException("not implemented");
#endif
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