using System;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WhiteBoardView _whiteboard;
        private string theme = "theme1";
        //uncomment below lines after the respective user controls are done
        /*private static ChatView _chat;
        private static UsersList _userslist;*/
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            _whiteboard = new WhiteBoardView();
            this.SSwb.Content = _whiteboard;

            //uncomment below lines after the respective User Controls are done
            /*_chat = new ChatView();
            this.Chat.Content = _chat;
            _userslist = new UsersList(this);
            this.UsersListControl.Content = _userslist;*/
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
        /// Function to change the theme
        /// </summary>
        private void OnThemeClick(object sender, RoutedEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            if (theme.Equals("theme1"))
            {
                theme = "theme2";
                dict.Source = new Uri("Theme2.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            else
            {
                theme = "theme1";
                dict.Source = new Uri("Theme1.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
        /// <summary>
        /// Function to handle ScreenShare button click event
        /// </summary>
        private void OnScreenShareClick(object sender, RoutedEventArgs e)
        {
            _ssFlag = true;   
            //uncomment below line after respective User Controls are done
            /*this.SSwb.Content = new ScreenShareView();
            if (_chatFlag.Equals(true) && _userslist.UserListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else if (_chatFlag.Equals(true))
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else if (_userslist.UserListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            }*/
        }
        /// <summary>
        /// Function to handle Whiteboard button click event
        /// </summary>
        private void OnWhiteboardClick(object sender, RoutedEventArgs e)
        {
            _wbFlag = true;
            this.SSwb.Content = _whiteboard;

            //uncomment below lines after the respective User Controls are done
            /*if (_chatFlag.Equals(true) && _userslist.UserListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else if (_chatFlag.Equals(true))
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else if (_userslist.UserListHidden.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                SSwb.SetValue(Grid.ColumnSpanProperty, 3);
            }
            else
            {
                SSwb.SetValue(Grid.ColumnProperty, 2);
                SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            }*/
        }
        /// <summary>
        /// Function to handle Chat button click event
        /// </summary>
        private void OnChatButtonClick(object sender, RoutedEventArgs e)
        {
            //uncomment below lines after the respective user controls are done

            /*if (_chatFlag.Equals(false))
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_userslist.UserListHidden.Equals(false))
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
                //uncomment after Chat user control is done
                this.Chat.Visibility = Visibility.Visible;
                _chatFlag = true;
            }
            else
            {
                this.Chat.Visibility = Visibility.Collapsed;
                _chatFlag = false;
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_userslist.UserListHidden.Equals(false))
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
            }*/
        }
        /// <summary>
        /// Function to handle Dashboard button click event
        /// </summary>
        private void OnDashboardClick(object sender, RoutedEventArgs e)
        {
            //uncomment after Dashboard is added 
            /*Dashboard dashboard = new Dashboard();
            dashboard.Show();*/
        }
        /// <summary>
        /// Function to handle UsersList expansion button click event
        /// </summary>
        public void OnUsersListClick()
        {
            //uncomment below lines after the respective User Controls are done
            /*if (_userslist.UserListHidden.Equals(true))
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    SSwb.SetValue(Grid.ColumnProperty, 4);
                    if (_chatFlag.Equals(true))
                    {                        
                        SSwb.SetValue(Grid.ColumnSpanProperty, 1);
                    }

                    else
                    {                      
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    }
                }
            }
            else
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    SSwb.SetValue(Grid.ColumnProperty, 2);
                    if (_chatFlag.Equals(true))
                    {
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    }
                    else
                    {
                        SSwb.SetValue(Grid.ColumnSpanProperty, 5);
                    }
                }
            }*/
        }
        /// <summary>
        /// Function to call OnLeaveButtonClick() when Leave button is clicked
        /// </summary>
        private void OnLeaveButtonClicked(object sender, RoutedEventArgs e)
        {
            //uncomment below line after UsersList User Control is done
            //_userslist.OnLeaveButtonClick();
        }

        private bool _chatFlag = false;
        private bool _ssFlag = false;
        private bool _wbFlag = true;
    }
}