using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Models
{
    /// <summary>
    /// Helper to allow loading data from storage.
    /// </summary>
    public class StorageContainer
    {
        private readonly IStorageProvider _storageProvider;
        private const string _myProfileDir = "profiles";
        private const string _contactsDir = "contacts";

        public StorageContainer(IStorageProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _storageProvider = provider;
        }

        /// <summary>
        /// Retrieves the received contacts from storage.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MyProfile>> GetReceivedContacts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a received contact to storage.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public async Task SaveReceivedContact(MyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var formatted = ConvertToRawText(profile);
            var path = GetUniquePath(profile, _contactsDir);

            _storageProvider.WriteAllText(path, formatted);
        }

        /// <summary>
        /// Gets the unique path for thew specific profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string GetUniquePath(MyProfile profile, string directory)
        {
            return $"/{directory}/{profile.ProfileName}.contact";
        }

        private string ConvertToRawText(MyProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}