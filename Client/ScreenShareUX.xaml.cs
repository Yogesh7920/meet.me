/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 26/10/2021
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
using ScreenSharing;
using Client.ViewModel;
using System.Diagnostics;

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

            Trace.WriteLine("[ScreenShareUX] instance is created");
            ScreenShareViewModel SSviewModel = new ScreenShareViewModel();
            this.DataContext = SSviewModel;
        }
    }
}
