/// <authors>Irene Casmir and P S Harikrishnan</authors>
/// <created>08/10/2021</created>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScreenSharing;

namespace Client
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WhiteBoardView s_whiteboard;
        private static ChatView s_chat;
        private static UsersList s_userslist;
        private static ScreenShareUX s_screenshare;
        public static bool s_sharing = false;
        private readonly ScreenShareClient _ssClient;
        private bool _chatFlag;
        private bool _ssFlag;
        private bool _wbFlag = true;

        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            s_whiteboard = new WhiteBoardView();
            s_screenshare = new ScreenShareUX();
            SSwb.Content = s_screenshare;
            SSwb.Content = s_whiteboard;
            s_chat = new ChatView();
            Chat.Content = s_chat;
            s_userslist = new UsersList(this);
            UsersListControl.Content = s_userslist;


            _ssClient = ScreenShareFactory.GetScreenShareClient();
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
        ///     Function to change the theme
        /// </summary>
        private void OnThemeClick(object sender, RoutedEventArgs e)
        {
            var dict = new ResourceDictionary();
            if (Theme.IsChecked == true)
            {
                dict.Source = new Uri("Theme2.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            else
            {
                dict.Source = new Uri("Theme1.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
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
            s_userslist.OnLeaveButtonClick();
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Function to handle ScreenShare button click event
        /// </summary>
        private void OnScreenShareClick(object sender, RoutedEventArgs e)
        {
            _ssFlag = true;
            SSwb.Content = s_screenshare;
            Trace.WriteLine("[MainWindow]" + s_sharing);
            if (!s_sharing)
                _ssClient.StartSharing();
            else
                _ssClient.StopSharing();

            //this.SSwb.Content = new ScreenShareUX();
            if (_chatFlag.Equals(true) && s_userslist.userListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else if (_chatFlag.Equals(true))
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else if (s_userslist.userListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            }
        }

        /// <summary>
        ///     Function to handle Whiteboard button click event
        /// </summary>
        private void OnWhiteboardClick(object sender, RoutedEventArgs e)
        {
            _wbFlag = true;
            SSwb.Content = s_whiteboard;
            if (_chatFlag.Equals(true) && s_userslist.userListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else if (_chatFlag.Equals(true))
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else if (s_userslist.userListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            }
        }

        /// <summary>
        ///     Function to handle Chat button click event
        /// </summary>
        private void OnChatButtonClick(object sender, RoutedEventArgs e)
        {
            if (_chatFlag.Equals(false))
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (s_userslist.userListHidden.Equals(false))
                    {
                        SSwb.SetValue(Grid.ColumnProperty, 4);
                        SSwb.SetValue(Grid.ColumnSpanProperty, 1);
                    }

                    else
                    {
                        SSwb.SetValue(Grid.ColumnProperty, 2);
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    }
                }

                Chat.Visibility = Visibility.Visible;
                _chatFlag = true;
            }
            else
            {
                Chat.Visibility = Visibility.Collapsed;
                _chatFlag = false;
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (s_userslist.userListHidden.Equals(false))
                    {
                        SSwb.SetValue(Grid.ColumnProperty, 4);
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    }
                    else
                    {
                        SSwb.SetValue(Grid.ColumnProperty, 2);
                        SSwb.SetValue(Grid.ColumnSpanProperty, 5);
                    }
                }
            }
        }

        /// <summary>
        ///     Function to handle Dashboard button click event
        /// </summary>
        private void OnDashboardClick(object sender, RoutedEventArgs e)
        {
            var dashboard = new DashboardView();
            dashboard.Show();
        }

        /// <summary>
        ///     Function to handle UsersList expansion button click event
        /// </summary>
        public void OnUsersListClick()
        {
            if (s_userslist.userListHidden.Equals(true))
            {
                UsersListControl.SetValue(Grid.ColumnSpanProperty, 3);
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    SSwb.SetValue(Grid.ColumnProperty, 4);
                    if (_chatFlag.Equals(true))
                        SSwb.SetValue(Grid.ColumnSpanProperty, 1);
                    else
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                }
            }
            else
            {
                UsersListControl.SetValue(Grid.ColumnSpanProperty, 1);
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    SSwb.SetValue(Grid.ColumnProperty, 2);
                    if (_chatFlag.Equals(true))
                    {
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    }
                    else
                    {
                        SSwb.SetValue(Grid.ColumnProperty, 2);
                        SSwb.SetValue(Grid.ColumnSpanProperty, 6);
                    }
                }
            }
        }

        /// <summary>
        ///     Function to call OnLeaveButtonClick() when Leave button is clicked
        /// </summary>
        private void OnLeaveButtonClicked(object sender, RoutedEventArgs e)
        {
            s_userslist.OnLeaveButtonClick();
            Application.Current.Shutdown();
        }
    }
}