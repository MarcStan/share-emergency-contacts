using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Models
{
    /// <summary>
    /// Low level file system wrapper.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Writes the specific file to storage.
        /// </summary>
        /// <param name="filePath">The filepath relative to the storage root.</param>
        /// <param name="text"></param>
        Task WriteAllTextAsync(string filePath, string text);

        /// <summary>
        /// Reads the specific file from storage.
        /// </summary>
        /// <param name="filePath">The filepath relative to the storage root.</param>
        /// <returns></returns>
        Task<string> ReadAllTextAsync(string filePath);

        /// <summary>
        /// Returns the path of all files inside the provided directory.
        /// </summary>
        /// <param name="directory">The filepath relative to the storage root.</param>
        /// <param name="pattern">Optional pattern to filter files. May contain * as a placeholder.</param>
        /// <returns></returns>
        Task<IReadOnlyList<string>> GetFilesAsync(string directory, string pattern = null);

        /// <summary>
        /// Returns the lines of the provided file.
        /// </summary>
        /// <param name="filePath">The filepath relative to the storage root.</param>
        /// <returns></returns>
        Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath);

        /// <summary>
        /// Deletes the specific file from disk.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        Task DeleteFileAsync(string filePath);
    }
}