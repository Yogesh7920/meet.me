/// <author>Irene Casmir</author>
/// <created>25/10/2021</created>

using System;
using System.Windows;
using System.Windows.Input;
using Client.ViewModel;

namespace Client
{
    public partial class AuthView : Window
    {
        private readonly MainWindow obj = new();

        public AuthView()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            var viewmodel = new AuthViewModel();
            DataContext = viewmodel;
        }

        //taken from https://stackoverflow.com/questions/4019831/how-do-you-center-your-main-window-in-wpf
        /// <summary>
        ///     Function to launch the window on the center of the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = Width;
            var windowHeight = Height;
            Left = screenWidth / 2 - windowWidth / 2;
            Top = screenHeight / 2 - windowHeight / 2;
        }

        /// <summary>
        ///     Drag functionality
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        ///     Minimize button functionality
        /// </summary>
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        /// <summary>
        ///     Maximize button functionality
        /// </summary>
        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            WindowState = WindowState.Maximized;
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Restore button functionality
        /// </summary>
        private void OnRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Close button functionality
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Handler method for Join Room button click
        /// </summary>
        private void OnJoinClick(object sender, RoutedEventArgs e)
        {
            var ip = IpBox.Text;
            var port = PortNumberBox.Text;
            var username = UsernameBox.Text;

            if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port) ||
                string.IsNullOrWhiteSpace(username) || int.TryParse(port, out _) == false)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                IpBox.Text = string.Empty;
                PortNumberBox.Text = string.Empty;
                UsernameBox.Text = string.Empty;
            }
            else
            {
                var viewmodel = DataContext as AuthViewModel;
                var result = viewmodel.SendForAuth(ip, Convert.ToInt32(port), username);
                if (result)
                {
                    obj.Show();
                    Close();
                }
                else
                {
                    ErrorMsg.Visibility = Visibility.Visible;
                    IpBox.Text = string.Empty;
                    PortNumberBox.Text = string.Empty;
                    UsernameBox.Text = string.Empty;
                }
            }
        }
    }
}