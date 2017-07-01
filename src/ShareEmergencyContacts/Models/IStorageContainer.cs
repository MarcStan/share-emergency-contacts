using ShareEmergencyContacts.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Models
{
    /// <summary>
    /// Storage container tailored to the current application.
    /// </summary>
    public interface IStorageContainer
    {
        /// <summary>
        /// Retrieves the received contacts from storage.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EmergencyProfile>> GetReceivedContactsAsync();

        /// <summary>
        /// Retrieves the profiles of the current app user from storage.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EmergencyProfile>> GetProfilesAsync();

        /// <summary>
        /// Saves a received contact to storage.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task SaveReceivedContactAsync(EmergencyProfile profile);

        /// <summary>
        /// Deletes the received profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task DeleteReceivedContactAsync(EmergencyProfile profile);

        /// <summary>
        /// Deletes the user profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        Task DeleteProfileAsync(EmergencyProfile profile);

        /// <summary>
        /// Saves a new profile.
        /// </summary>
        /// <param name="newContact"></param>
        /// <returns></returns>
        Task SaveProfileAsync(EmergencyProfile newContact);
    }
}