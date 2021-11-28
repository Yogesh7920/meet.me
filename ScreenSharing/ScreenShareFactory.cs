/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 25/11/2021
 * date modified: 27/11/2021
**/
using System;
using Networking;

namespace ScreenSharing
{
    /// <summary>
    /// Factory for the ScreenShareClient object.
    /// </summary>
    public static class ScreenShareFactory
    {
        private static readonly Lazy<ScreenShareClient> s_clientScreenSharer = new Lazy<ScreenShareClient>(() => new ScreenShareClient());
        private static readonly Lazy<ScreenShareServer> s_serverScreenSharer = new Lazy<ScreenShareServer>(() => new ScreenShareServer());

        public static ScreenShareClient GetScreenShareClient()
        {
           return s_clientScreenSharer.Value;        
        }

        public static ScreenShareServer GetScreenShareServer()
        {
            return s_serverScreenSharer.Value;
        }
    }
}
