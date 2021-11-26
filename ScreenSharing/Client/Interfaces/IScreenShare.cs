/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 14/10/2021
**/

using System.Drawing;

namespace ScreenSharing
{
    /// <summary>
    ///     interface to be implemented by subscribers of screenshareclient.
    /// </summary>
    public interface IScreenShare
    {
        /// <summary>
        ///     handles the reception of update from screen share client.
        /// </summary>
        /// <param name="screen">SharedScreen type object.</param>
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen);
    }
}