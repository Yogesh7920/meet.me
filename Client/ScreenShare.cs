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
        public int userId;

        //Stores the user name of the user sharing the screen
        public string username;
        public void onScreenRecieved(SharedScreen screen)
        {
            messageType = screen.messageType;
            sharedscreen = screen.screen;
            userId = screen.userId;
            username = screen.username;
        }
    }
}
