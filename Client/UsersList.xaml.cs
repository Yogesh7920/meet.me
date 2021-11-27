/// <author>P S Harikrishnan</author>
/// <created>06/11/2021</created>

using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Client.ViewModel;

namespace Client
{
    /// <summary>
    /// Interaction logic for UsersList.xaml
    /// </summary>
    public partial class UsersList : UserControl
    {
        public bool UserListHidden;
        ObservableCollection<UserViewData> users;
        MainWindow obj;
        HomePageViewModel viewModelHomePage;
        public UsersList(MainWindow instance)
        {
            InitializeComponent();
            obj = instance;

            UserListHidden = true;
            UserListHead.Visibility = System.Windows.Visibility.Hidden;

            viewModelHomePage = new HomePageViewModel();
            //subscribe to the property changed event
            viewModelHomePage.UsersListChanged += Listener;
            DataContext = viewModelHomePage;

            users = new ObservableCollection<UserViewData>();
            UsersListView.ItemsSource = users;
        }
        /// <summary>
        /// Listening to property changed event
        /// Sets new object as ItemSource
        /// </summary>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            users = new ObservableCollection<UserViewData>(viewModelHomePage.users);
            UsersListView.ItemsSource = users;
        }
        /// <summary>
        /// Resizing users list based on button click
        /// </summary>
        private void UsersListClick(object sender, RoutedEventArgs e)
        {
            obj.OnUsersListClick();
            if (UserListHidden)
            {
                UsersListPane.SetValue(Grid.ColumnSpanProperty, 3);
                UserListHead.Visibility = System.Windows.Visibility.Visible;
                UserListHidden = false;
            }
            else
            {
                UsersListPane.SetValue(Grid.ColumnSpanProperty, 1);
                UserListHead.Visibility = System.Windows.Visibility.Hidden;
                UserListHidden = true;
            }
        }
        /// <summary>
        /// Leave button clicked
        /// </summary>
        public void OnLeaveButtonClick()
        {
            viewModelHomePage.LeftClient();
        }
    }
}
