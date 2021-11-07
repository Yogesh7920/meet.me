/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 7/11/2021
**/

using System;
using System.Drawing;

namespace ScreenSharing
{
    /// <summary>
    /// interface to be implemented by subscribers of screenshareclient.
    /// </summary>
    public interface IScreenShare
    {
        /// <summary>
        /// handles the reception of update from screen share client. 
        /// </summary>
        /// <param name="screen">SharedScreen type object.</param>
        public void onScreenRecieved(string uid,string uname,Bitmap screen);
    }
}
