/**
 * Owned By: Pulavarti Vinay Kumar
 * Created By: Neeraj
 * Date Created: 10 Oct 2021
 * Date Modified: 16 Nov 2021
**/

<<<<<<< HEAD
using System;
using System.Drawing;

=======
>>>>>>> f1e8300bf03f574434695ee951fbc63d2a40a3ff
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
        /// <param name="uid"> Stores the Id of the user sharing the screen.</param>
        /// <param name="uname"> Stores the username of the user sharing the screen.</param>
        /// <param name="mtype"> Stores the message type.</param>
        /// <param name="screen"> Stores the screen in the Bitmap format.</param>
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen);
    }
}