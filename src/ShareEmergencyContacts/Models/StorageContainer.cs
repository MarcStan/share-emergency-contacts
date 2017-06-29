using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Models
{
    /// <summary>
    /// Helper to allow work with platform independent storage to load/save profiles and emergency contacts.
    /// </summary>
    public class StorageContainer : IStorageContainer
    {
        private readonly IStorageProvider _storageProvider;
        private const string _myProfileDir = "profiles";
        private const string _contactsDir = "contacts";

        public StorageContainer(IStorageProvider provider)
        {
            _storageProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Retrieves the received contacts from storage.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EmergencyProfile>> GetReceivedContactsAsync()
        {
            var files = await _storageProvider.GetFilesAsync(_contactsDir, "*.contact");
            return await FromFilesAsync(files);
        }

        /// <summary>
        /// Retrieves the profiles of the current app user from storage.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EmergencyProfile>> GetProfilesAsync()
        {
            var files = await _storageProvider.GetFilesAsync(_myProfileDir, "*.contact");
            return await FromFilesAsync(files);
        }

        /// <summary>
        /// Internal helper to load contact files from disk.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private async Task<IEnumerable<EmergencyProfile>> FromFilesAsync(IEnumerable<string> files)
        {
            var contacts = new List<EmergencyProfile>();
            foreach (var f in files)
            {
                var content = await _storageProvider.ReadAllTextAsync(f);
                var c = EmergencyProfile.ParseFromText(content);
                if (c.ExpirationDate.HasValue && c.ExpirationDate < DateTime.Now)
                {
                    // contact expired, delete
                    await _storageProvider.DeleteFileAsync(f);
                }
                else
                {
                    contacts.Add(c);
                }
            }
            return contacts;
        }

        /// <summary>
        /// Saves a received contact to storage.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public async Task SaveReceivedContact(EmergencyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var formatted = ConvertToRawText(profile);
            var path = GetUniquePath(profile, _contactsDir);

            await _storageProvider.WriteAllTextAsync(path, formatted);
        }

        /// <summary>
        /// Gets the unique path for thew specific profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetUniquePath(EmergencyProfile profile, string directory)
        {
            return $"/{directory}/{profile.ProfileName}.contact";
        }

        private string ConvertToRawText(EmergencyProfile profile)
        {
            return EmergencyProfile.ToRawText(profile);
        }
    }
}