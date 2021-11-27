/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 12/10/2021
 * date modified: 22/10/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ScreenSharing;

namespace Client
{
    class ScreenShare : IScreenShare
    {
        //Stores the type of message
        public int messageType;

        //Stores the screen
        public Bitmap sharedscreen;

        //Stores the user Id of the user sharing the screen
        public string userId;

        //Stores the user name of the user sharing the screen
        public string username;
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            messageType = mtype;
            sharedscreen = screen;
            userId = uid;
            username = uname;
        }
    }
}
