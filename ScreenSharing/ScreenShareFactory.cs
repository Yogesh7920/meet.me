/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 25/11/2021
 * date modified: 25/11/2021
**/
using System;

namespace ScreenSharing
{
    /// <summary>
    /// Factory for the ScreenShareClient object.
    /// </summary>
    public sealed class ScreenShareFactory
    {
        private static readonly Lazy<ScreenShareClient> ScreenSharer = new Lazy<ScreenShareClient>(() => new ScreenShareClient());
        public static ScreenShareClient GetScreenSharer(bool isTesting = false)
        {
            return isTesting? new ScreenShareClient() : ScreenSharer.Value;
            
        }
    }
}
