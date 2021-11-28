/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 16/10/2021
 * date modified: 26/10/2021
**/

using System.Diagnostics;
using System.Windows.Controls;
using Client.ViewModel;

namespace Client
{
    /// <summary>
    ///     Interaction logic for ScreenShareUX.xaml
    /// </summary>
    public partial class ScreenShareUX : UserControl
    {
        public ScreenShareUX()
        {
            InitializeComponent();

            Trace.WriteLine("[ScreenShareUX] instance is created");
            var SSviewModel = new ScreenShareViewModel();
            DataContext = SSviewModel;
        }
    }
}