/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 14/10/2021
**/

using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace ScreenSharing
{

    public class SharedScreen
    {
        //Stores the user Id of the user sharing the screen
        public int userId;

        //Stores the user name of the user sharing the screen
        public string username;

        //Stores the type of message
        public int messageType;

        //Stores the screen
        public Bitmap screen;
    }
}
