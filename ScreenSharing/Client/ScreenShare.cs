/**
 * Owned By: Pulavarti Vinay Kumar
 * Created By: Pulavarti Vinay Kumar
 * Date Created: 16 Nov 2021
 * Date Modified: 17 Nov 2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ScreenSharing;

namespace ScreenSharing
{
    public class ScreenShare : IScreenShare
    {
        public string userid;
        public string username;
        public int _mtype;
        public Bitmap _screen;
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            userid = "456";
            username = "vinay";
            _mtype = mtype;
            _screen = screen;


            // Problem in the network connection
            //if (mtype == -2)
            //{
            //    Console.WriteLine("You have lost ur network connection");
            //}
            //else if (mtype == -1) // some one else is sharing so u can't share ur screen
            //{
            //    Console.WriteLine("some one else is sharing so u can't share ur screen");
            //}
            //else if (mtype == 0)  // Stop the screen share
            //{

            //}
            //else if (mtype == 1)  // screen share is going now
            //{

            //}

        }
    }
}
