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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.ViewModels;

namespace Client
{
    public partial class AuthView : Window
    {
        private MainWindow obj = new MainWindow();
        public AuthView()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            AuthViewModel viewmodel = new AuthViewModel();
            this.DataContext = viewmodel;
        }
        //taken from https://stackoverflow.com/questions/4019831/how-do-you-center-your-main-window-in-wpf
        /// <summary>
        /// Function to launch the window on the center of the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        /// <summary>
        /// Handler method for Join Room button click
        /// </summary>
        private void OnJoinClick(object sender, RoutedEventArgs e)
        {
           var ip = this.IpBox.Text;
           var port = this.PortNumberBox.Text;
           var username = this.UsernameBox.Text;

           if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port) ||
               string.IsNullOrWhiteSpace(username) || int.TryParse(port, out _) == false)
           {
               ErrorMsg.Visibility = System.Windows.Visibility.Visible;
               this.IpBox.Text = String.Empty;
               this.PortNumberBox.Text = String.Empty;
               this.UsernameBox.Text = String.Empty;
            }
           else
           {
               AuthViewModel viewmodel = this.DataContext as AuthViewModel;
               var result = viewmodel.SendForAuth(ip, Convert.ToInt32(port), username);
               if (result == true)
               {
                   obj.Show();
                   Close();
               }
               else
               {
                   ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                   this.IpBox.Text = String.Empty;
                   this.PortNumberBox.Text = String.Empty;
                   this.UsernameBox.Text = String.Empty;
                }
           }

        }
       
    }
}