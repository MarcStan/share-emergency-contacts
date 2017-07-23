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
    }
}
