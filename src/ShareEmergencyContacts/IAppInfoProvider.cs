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
    }
}
