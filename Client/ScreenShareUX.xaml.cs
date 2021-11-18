using System;
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
using ScreenSharing;

namespace Client
{
    /// <summary>
    /// Interaction logic for ScreenShareUX.xaml
    /// </summary>
    public partial class ScreenShareUX : Window
    {
        public ScreenShareUX()
        {
            InitializeComponent();

            ScreenShare Screenviewmodel = new ScreenShare();
            this.DataContext = Screenviewmodel;

            Console.WriteLine(Screenviewmodel.userid);

            // Problem in the network connection
            if (Screenviewmodel._mtype == -2)
            {
                Console.WriteLine("You have lost ur network connection");
            }
            else if (Screenviewmodel._mtype == -1) // some one else is sharing so u can't share ur screen
            {
                Console.WriteLine("some one else is sharing so u can't share ur screen");
            }
            else if (Screenviewmodel._mtype == 0)  // Stop the screen share
            {

            }
            else if (Screenviewmodel._mtype == 1)  // screen share is going now
            {

            }

        }
    }
}
