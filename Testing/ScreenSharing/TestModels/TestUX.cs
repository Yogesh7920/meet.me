using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenSharing;

namespace Testing.ScreenSharing.TestModels
{
    class TestUX : IScreenShare
    {
        public string UserId;
        public string UserName;
        public int MessageType;
        public Bitmap Screen;
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            UserId = uid;
            UserName = uname;
            MessageType = mtype;
            Screen = screen;
        }
    }
}
