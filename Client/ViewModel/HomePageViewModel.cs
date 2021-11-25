using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Client;

namespace Client.ViewModel
{

    class HomePageViewModel : IClientSessionNotifications
    {
        private IUXClientSessionManager _model;

        public List<UserViewData> users
        {
            get; private set;
        }
        public HomePageViewModel()
        {
            _model = SessionManagerFactory.GetClientSessionManager();
            _model.SubscribeSession(this);
            users = new List<UserViewData>();
        }

        public void OnClientSessionChanged(SessionData session)
        {
            int userid = ChatViewModel.UserId;
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<SessionData, List<UserViewData>,int>((session,users,userid) =>
                        {
                            lock (this)
                            {
                                users.Clear();
                                foreach (UserData user in session.users)
                                {
                                    UserViewData usernew = new UserViewData();
                                    usernew.username = user.username;
                                    if(user.userID == userid)
                                    {
                                        usernew.username += " (You)";
                                    }
                                    usernew.shortname = user.username.Substring(0,2);
                                    users.Add(usernew);
                                    System.Diagnostics.Debug.WriteLine(user.username);
                                }
                                OnPropertyChanged("ListChanged");
                            }
                        }),
                        session,users,userid);
        }

        public void LeftClient()
        {
            _model.RemoveClient();
        }

        public event PropertyChangedEventHandler UsersListChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void OnPropertyChanged(string property)
        {
            UsersListChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;
    }
}
