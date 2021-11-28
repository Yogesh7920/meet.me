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
        public bool userListHidden;
        ObservableCollection<UserViewData> users;
        MainWindow obj;
        private HomePageViewModel _viewModelHomePage;
        public UsersList(MainWindow instance)
        {
            InitializeComponent();
            obj = instance;

            userListHidden = true;
            UserListHead.Visibility = System.Windows.Visibility.Hidden;

            _viewModelHomePage = new HomePageViewModel();
            //subscribe to the property changed event
            _viewModelHomePage.UsersListChanged += Listener;
            DataContext = _viewModelHomePage;

            users = new ObservableCollection<UserViewData>();
            UsersListView.ItemsSource = users;
        }
        /// <summary>
        /// Listening to property changed event
        /// Sets new object as ItemSource
        /// </summary>
        private void Listener(object sender, PropertyChangedEventArgs e)
        {
            users = new ObservableCollection<UserViewData>(_viewModelHomePage.users);
            UsersListView.ItemsSource = users;
        }
        /// <summary>
        /// Resizing users list based on button click
        /// </summary>
        private void UsersListClick(object sender, RoutedEventArgs e)
        {
            obj.OnUsersListClick();
            if (userListHidden)
            {
                UsersListPane.SetValue(Grid.ColumnSpanProperty, 3);
                UserListHead.Visibility = System.Windows.Visibility.Visible;
                userListHidden = false;
            }
            else
            {
                UsersListPane.SetValue(Grid.ColumnSpanProperty, 1);
                UserListHead.Visibility = System.Windows.Visibility.Hidden;
                userListHidden = true;
            }
        }
        /// <summary>
        /// Leave button clicked
        /// </summary>
        public void OnLeaveButtonClick()
        {
            _viewModelHomePage.LeftClient();
        }
    }
}
