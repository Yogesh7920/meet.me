/// <author>Irene Casmir</author>
/// <created>25/10/2021</created>

using Client.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

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
        /// Drag functionality
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        /// <summary>
        /// Minimize button functionality
        /// </summary>  
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        
        /// <summary>
        /// Maximize button functionality
        /// </summary>
        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.WindowState = WindowState.Maximized;
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// Restore button functionality
        /// </summary>
        private void OnRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }
        
        /// <summary>
        /// Close button functionality
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
               ErrorMsg.Visibility = Visibility.Visible;
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
                   ErrorMsg.Visibility = Visibility.Visible;
                   this.IpBox.Text = String.Empty;
                   this.PortNumberBox.Text = String.Empty;
                   this.UsernameBox.Text = String.Empty;
               }
           }

        }
       
    }
}