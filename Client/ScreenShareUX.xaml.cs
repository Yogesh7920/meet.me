/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 22/10/2021
**/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;

namespace Client
{
    /// <summary>
    /// Interaction logic for ScreenShareUX.xaml
    /// </summary>
    public partial class ScreenShareUX : UserControl
    {
        public ScreenShareUX()
        {
            InitializeComponent();

            ScreenShare screenviewmodel = new ScreenShare();
            this.DataContext = screenviewmodel;

            while (true)
            {
                // Problem in the network connection
                if (screenviewmodel.messageType == -2)
                {
                    File.Delete("../../../Icons/screenshare.png");
                    File.Copy("../../../Icons/meet.me_logo_no_bg.png", "../../../Icons/screenshare.png");

                    this.ReceivedMessage.Text = "Network Problem";
                }
                else if (screenviewmodel.messageType == -1) // some one else is sharing so u can't share ur screen
                {
                    File.Delete("../../../Icons/screenshare.png");
                    File.Copy("../../../Icons/meet.me_logo_no_bg.png", "../../../Icons/screenshare.png");

                    this.ReceivedMessage.Text = "some one else is sharing so u can't share ur screen";
                }
                else if (screenviewmodel.messageType == 0)  // Stop the screen share
                {
                    File.Delete("../../../Icons/screenshare.png");
                    File.Copy("../../../Icons/meet.me_logo_no_bg.png", "../../../Icons/screenshare.png");

                    this.ReceivedMessage.Text = "No one is sharing the screen";
                }
                else if (screenviewmodel.messageType == 1)  // screen share is going now
                {
                    Bitmap img = screenviewmodel.sharedscreen;
                    img.Save("../../../Icons/screenshare.png", ImageFormat.Png);

                    this.ReceivedMessage.Text = string.Empty;
                }
                else
                {
                    File.Delete("../../../Icons/screenshare.png");
                    File.Copy("../../../Icons/meet.me_logo_no_bg.png", "../../../Icons/screenshare.png");

                    this.ReceivedMessage.Text = "No one is sharing the screen";
                }
            }
        }
    }
}
