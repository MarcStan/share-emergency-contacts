namespace ShareEmergencyContacts
{
    /// <summary>
    /// Interface to provide values that need to be obtained differently per platform.
    /// </summary>
    public interface IAppInfoProvider
    {
        /// <summary>
        /// Gets the width of the screen in portrait.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        /// Gets the height of the screen in portrait.
        /// </summary>
        int ScreenHeight { get; }

        /// <summary>
        /// API key for mobile center valid for the current platform <see cref="Xamarin.Forms.Device.RuntimePlatform"/> only.
        /// </summary>
        string MobileCenterKey { get; }

        /// <summary>
        /// Gets whether the current system theme is dark.
        /// For iOS and android this will always return false since they don't support system-wide themes.
        /// For UWP this will return whether the system theme is dark or light (a new system install will always default to light).
        /// </summary>
        bool SystemThemeIsDark { get; }
    }
}
