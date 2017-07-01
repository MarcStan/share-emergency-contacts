using System;

namespace ShareEmergencyContacts
{
    /// <summary>
    /// Interface to provide values that need to be obtained differently per platform.
    /// </summary>
    public interface IAppInfoProvider
    {
        /// <summary>
        /// Gets the user friendly string version.
        /// This could either just be the version.ToString() or a random string (e.g. "v1.0-beta2")
        /// </summary>
        string UserFriendlyVersion { get; }

        /// <summary>
        /// Returns the actual app version.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the width of the screen in portrait.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// Gets the height of the screen in portrait.
        /// </summary>
        int ScreenHeight { get; }
    }
}
