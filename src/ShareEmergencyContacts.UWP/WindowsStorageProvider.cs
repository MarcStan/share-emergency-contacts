using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using CreationCollisionOption = Windows.Storage.CreationCollisionOption;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsStorageProvider : ShareEmergencyContacts.Models.IStorageProvider
    {
        public async Task WriteAllTextAsync(string filePath, string text)
        {
            var tuple = await GetFolderAndFileName(filePath);
            var dir = tuple.Item1;
            var fileName = tuple.Item2;

            var file = await dir.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
        }

        /// <summary>
        /// Helper that returns the directory and the filename from the provided path.
        /// If path contains no slahes it is assumed it is only a filename and the root directory is returned.
        /// </summary>
        /// <param name="filePath">A path relative to the root to a file.</param>
        /// <returns>The directory and the filename from the provided input value.</returns>
        private static async Task<Tuple<StorageFolder, string>> GetFolderAndFileName(string filePath)
        {
            var dir = ApplicationData.Current.LocalFolder;
            var fileName = filePath;
            if (filePath.Contains("\\") || filePath.Contains("/"))
            {
                // if we have subdir we need to create it if it doesn't exist
                filePath = filePath.Replace("/", "\\");
                if (filePath.StartsWith("\\"))
                    filePath = filePath.Substring(1);

                var last = filePath.LastIndexOf('\\');
                var dirPath = filePath.Substring(0, last);
                fileName = filePath.Substring(last + 1);
                dir = await dir.CreateFolderAsync(dirPath, CreationCollisionOption.OpenIfExists);
            }
            return new Tuple<StorageFolder, string>(dir, fileName);
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            var tuple = await GetFolderAndFileName(filePath);
            var dir = tuple.Item1;
            var fileName = tuple.Item2;

            var file = await dir.GetFileAsync(fileName);
            string text = await FileIO.ReadTextAsync(file);
            return text;
        }

        public async Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null)
        {
            var patternMatch = pattern == null ?
                new Func<string, bool>(f => true) : f =>
                {
                    if (pattern.Contains("*"))
                    {
                        var regex = new Regex(pattern.Replace(".", "\\.").Replace("*", ".*"));
                        return regex.IsMatch(f);
                    }
                    return pattern.Equals(f, StringComparison.CurrentCultureIgnoreCase);
                };

            var storageFolder = ApplicationData.Current.LocalFolder;
            var di = new DirectoryInfo(Path.Combine(storageFolder.Path, directory));
            if (!di.Exists)
                return new List<string>();

            var dir = await storageFolder.GetFolderAsync(directory);
            var files = await dir.GetFilesAsync(CommonFileQuery.DefaultQuery);
            return files.Where(f => patternMatch(f.Name)).Select(f => Path.Combine(directory, f.Name)).ToImmutableList();
        }

        public async Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            var tuple = await GetFolderAndFileName(filePath);
            var dir = tuple.Item1;
            var fileName = tuple.Item2;

            var file = await dir.GetFileAsync(fileName);
            var lines = await FileIO.ReadLinesAsync(file);
            return lines.ToImmutableList();
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var tuple = await GetFolderAndFileName(filePath);
            var dir = tuple.Item1;
            var fileName = tuple.Item2;
            try
            {
                var file = await dir.GetFileAsync(fileName);
                await file.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
            }
        }

        public async Task<bool> SaveExternallyAsync(string filename, string content)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };
            var ext = Path.GetExtension(filename);
            savePicker.FileTypeChoices.Add(ext, new List<string> { ext });
            savePicker.SuggestedFileName = filename;
            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return false;

            using (var stream = await file.OpenStreamForWriteAsync())
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(content);
            }
            return true;
        }

        public async Task<string> ReadExternallyAsync(string ext)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };
            openPicker.FileTypeFilter.Add(ext);
            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
                return null;
            using (var stream = await file.OpenStreamForReadAsync())
            using (var reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }
    }
}