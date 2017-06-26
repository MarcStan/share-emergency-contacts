﻿using System;

namespace ShareEmergencyContacts
{
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
    }
}
