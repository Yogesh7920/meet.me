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
        }
    }
}
