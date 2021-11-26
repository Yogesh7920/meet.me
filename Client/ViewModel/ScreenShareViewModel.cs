/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 26/10/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenSharing;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using System.Diagnostics;

namespace Client.ViewModel
{
    class ScreenShareViewModel : IScreenShare
    {
        /// <summary>
        /// The received message.
        /// </summary>
        public string ReceivedMessage
        {
            get; private set;
        }

        public ScreenShareViewModel()
        {
            ScreenShareClient model = ScreenShareFactory.GetScreenSharer();
            model.Subscribe(this);

            this.ReceivedMessage = "No one is sharing the screen(Default)";
        }

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        public void OnScreenRecieved(string uid, string uname, int mtype, Bitmap screen)
        {
            string home = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = String.Join(@"\", home.Split('\\').Reverse().Skip(3).Reverse());
            //Debug.WriteLine(path);
            // Problem in the network connection
            if (mtype == -2)
            {
                File.Delete(path + "/Icons/screenshare.png");
                File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                this.ReceivedMessage = "Network Problem";
            }
            else if (mtype == -1) // some one else is sharing so u can't share ur screen
            {
                File.Delete(path + "/Icons/screenshare.png");
                File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                this.ReceivedMessage = "some one else is sharing so u can't share ur screen";
            }
            else if (mtype == 0)  // Stop the screen share
            {
                File.Delete(path + "/Icons/screenshare.png");
                File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                this.ReceivedMessage = "No one is sharing the screen";
            }
            else if (mtype == 1)  // screen share is going now
            {
                screen.Save(path + "/Icons/screenshare.png", ImageFormat.Png);

                this.ReceivedMessage = string.Empty;
            }
            else
            {
                File.Delete(path + "/Icons/screenshare.png");
                File.Copy(path + "/Icons/meet.me_logo_no_bg.png", path + "/Icons/screenshare.png");

                this.ReceivedMessage = "No one is sharing the screen";
            }
        }
    }
}
