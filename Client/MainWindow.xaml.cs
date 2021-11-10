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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WhiteBoardView _whiteboard ;
        //
        
        //
        //private static Chat _chat;
        private static UsersList _userslist;
        //private ScreenShare _screenshare;
        public MainWindow()
        {
            InitializeComponent();
            //
            //
            CenterWindowOnScreen();
            _whiteboard = new WhiteBoardView();
            this.SSwb.Content = _whiteboard;

            //uncomment below lines after the respective user controls are done
           // _chat = new Chat();
           // _screenshare = new ScreenShare();
           // this.Chat.Content = _chat;

            _userslist = new UsersList();
            this.UsersListControl.Content = _userslist;
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }


        /*private void Users_List_Clicked(object sender, RoutedEventArgs e)
        {
            if (_usersListFlag.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_chatFlag.Equals(true))
                        SSwb.SetValue(Grid.ColumnSpanProperty, 1);
                    else
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                }
                UsersListPane2.Visibility = Visibility.Visible;
                _usersListFlag = true;
            }
            else
            {
                UsersListPane2.Visibility = Visibility.Collapsed;
                _usersListFlag = false;
                SSwb.SetValue(Grid.ColumnProperty, 2);
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_chatFlag.Equals(true))
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                    else
                        SSwb.SetValue(Grid.ColumnSpanProperty, 5);
                }
            }
        }*/

        private void ScreenShare_Clicked(object sender, RoutedEventArgs e)
        {
            /*DataContext = new ScreenShareViewModel();
            _ssFlag = true;
            if (_chatFlag.Equals(true))
            {
                ChatPane.Visibility = System.Windows.Visibility.Collapsed;
                _chatFlag = false;

            }
            if (_usersListFlag.Equals(true))
            {
                UsersListPane2.Visibility = System.Windows.Visibility.Collapsed;
                _usersListFlag = false;
            }
            SSwb.SetValue(Grid.ColumnProperty, 2);
            SSwb.SetValue(Grid.ColumnSpanProperty, 5);*/

            //ACTUAL IMPLEMENTATION
            _ssFlag = true;   //uncomment below line after screenshare user control is done
            //this.SSwb.Content = _screenshare;
            if (_chatFlag.Equals(true))
            {
                Chat.Visibility = Visibility.Collapsed;
                _chatFlag = false;

            }
            if (_usersListFlag.Equals(true))
            {
                //UsersListPane2.Visibility = System.Windows.Visibility.Collapsed;
                _usersListFlag = false;
            }
            SSwb.SetValue(Grid.ColumnProperty, 3);
            SSwb.SetValue(Grid.ColumnSpanProperty, 1);
        }

        private void Whiteboard_Clicked(object sender, RoutedEventArgs e)
        {
            /*DataContext = new WhiteBoardViewModel();
            _wbFlag = true;
            if (_chatFlag.Equals(true))
            {
                ChatPane.Visibility = System.Windows.Visibility.Collapsed;
                _chatFlag = false;

            }
            if (_usersListFlag.Equals(true))
            {
                UsersListPane2.Visibility = System.Windows.Visibility.Collapsed;
                _usersListFlag = false;
            }
            SSwb.SetValue(Grid.ColumnProperty, 2);
            SSwb.SetValue(Grid.ColumnSpanProperty, 5);*/

            //ACTUAL IMPLEMENTATION
            _wbFlag = true;
            this.SSwb.Content = _whiteboard;
            if (_chatFlag.Equals(true))
            {
                Chat.Visibility = Visibility.Collapsed;
                _chatFlag = false;

            }
            if (_usersListFlag.Equals(true))
            {
                //UsersListPane2.Visibility = Visibility.Collapsed;
                _usersListFlag = false;
            }
            SSwb.SetValue(Grid.ColumnProperty, 3);
            SSwb.SetValue(Grid.ColumnSpanProperty, 1);
        }

        private void Chat_Button_Clicked(object sender, RoutedEventArgs e)
        {

            // DataContext = new ChatViewModel();
            /*
            if (_chatFlag.Equals(false))
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_usersListFlag.Equals(true))
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

                ChatPane.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {

                ChatPane.Visibility = System.Windows.Visibility.Collapsed;
                _chatFlag = false;
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_usersListFlag.Equals(true))
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

            //ACTUAL IMPLEMENTATION
            if (_chatFlag.Equals(false))
            {
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if (_usersListFlag.Equals(true))
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
                    if (_usersListFlag.Equals(true))
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

        public void Dashboard_Clicked(object sender, RoutedEventArgs e)
        {
            /*Dashboard dashboard = new Dashboard();
            dashboard.Show();*/
        }
        private void OnLeaveButtonClick(object sender, RoutedEventArgs e)
        {
            HomePageViewModel homeviewmodel = this.DataContext as HomePageViewModel;
            homeviewmodel.LeftClient();
        }

        private bool _usersListFlag = false;
        private bool _chatFlag = false;
        private bool _ssFlag = false;
        private bool _wbFlag = true;


    }
}