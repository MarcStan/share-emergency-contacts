using ShareEmergencyContacts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#if __ANDROID__
using Android.Provider;
using Android.App;
using Android.Content;
using Android.Content.PM;
#endif

namespace ShareEmergencyContacts.Droid
{
    public class AndroidIOSStorageProvider : IStorageProvider
    {
#if __ANDROID__
        private readonly Dictionary<int, TaskCompletionSource<string>> _tokens;
        private int _nextFreeCode;
        private readonly MainActivity _activity;

        public AndroidIOSStorageProvider(MainActivity activity)
        {
            _activity = activity;
            _tokens = new Dictionary<int, TaskCompletionSource<string>>();
        }
#endif

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

        public Task<bool> SaveExternallyAsync(string filename, string content)
        {
#if __ANDROID__
            return Task.Run(() =>
            {
                var dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                Directory.CreateDirectory(dir.AbsolutePath);
                File.WriteAllText(Path.Combine(dir.AbsolutePath, filename), content);
                return true;
            });
#else
            throw new NotSupportedException();
#endif
        }

        public async Task<string> ReadExternallyAsync(string ext)
        {
#if __ANDROID__
            var intent = new Intent(Intent.ActionOpenDocument);
            // To risky, not all androids seem to support custom filters for *.ext, so just allow all
            intent.SetType("*/*");

            Intent.CreateChooser(intent, "Select file");
            // retarded android bullshit: the result is not returned here but in the main activity
            // so we proxied it to the
            // use unique id to PermissionRequestAnswered which will need a unique token per request
            var id = _nextFreeCode;
            _nextFreeCode++;
            // finally store a task that completes once the answer is received
            var tcs = new TaskCompletionSource<string>();
            _tokens.Add(id, tcs);

            // run permission request by android (may or may not spawn dialog)
            try
            {
                _activity.StartActivityForResult(intent, id);
            }
            catch (Exception exAct)
            {
                System.Diagnostics.Debug.Write(exAct);
            }
            // await the result proxied to PermissionRequestAnswered
            await tcs.Task;
            var path = tcs.Task.Result;
            if (path == null)
                return null; // user canceled

            var docId = DocumentsContract.GetDocumentId(Android.Net.Uri.Parse(path));
            string[] split = docId.Split(':');
            var type = split[0];

            // SD card is not primary but some random semi-hash value
            if (!"primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Cannot read from sd card");

            // read from internal storage
            var path2 = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + split[1];
            try
            {
                var content = File.ReadAllText(path2);
                return content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
#else
            return null;
#endif
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

#if __ANDROID__

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (!_tokens.ContainsKey(requestCode))
                throw new NotSupportedException("Missing token for request code");

            var token = _tokens[requestCode];
            _tokens.Remove(requestCode);

            if (resultCode == Result.Ok && data.Data != null)
            {
                token.SetResult(data.DataString);
            }
            else
            {
                token.SetResult(null);
            }
        }
#endif
    }
}