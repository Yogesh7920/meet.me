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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          //  DataContext = new WhiteBoardViewModel();
        }


        private void Users_List_Clicked(object sender, RoutedEventArgs e)
        {

            if (_usersListFlag.Equals(false))
            {
                SSwb.SetValue(Grid.ColumnProperty, 4);
                if (_ssFlag.Equals(true) || _wbFlag.Equals(true))
                {
                    if(_chatFlag.Equals(true))
                        SSwb.SetValue(Grid.ColumnSpanProperty, 1);
                    else
                        SSwb.SetValue(Grid.ColumnSpanProperty, 3);
                }
                UsersListPane2.Visibility = System.Windows.Visibility.Visible;
                _usersListFlag = true;
            }
            else
            {
                
                UsersListPane2.Visibility = System.Windows.Visibility.Collapsed;
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
        }
        private void ScreenShare_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new ScreenShareViewModel();
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
            SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            
        }

        private void Whiteboard_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new WhiteBoardViewModel();
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
            SSwb.SetValue(Grid.ColumnSpanProperty, 5);
            
        }

        private void Chat_Button_Clicked(object sender, RoutedEventArgs e)
        {
            
               // DataContext = new ChatViewModel();

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

                ChatPane.Visibility = System.Windows.Visibility.Visible;
                _chatFlag = true;
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
            }
        }
        
        private void Dashboard_Clicked(object sender, RoutedEventArgs e)
        {
            //DataContext = new DashboardViewModel();
            
        }


        private bool _usersListFlag = false;
        private bool _chatFlag = false;
        private bool _ssFlag = false;
        private bool _wbFlag = true;
        

    }
}