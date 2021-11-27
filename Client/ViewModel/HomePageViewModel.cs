using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Threading;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Client;

namespace Client.ViewModel
{
    public class HomePageViewModel : IClientSessionNotifications // Notifies change in list of users.
    {
        int userid;
        public List<UserViewData> users
        {
            get; private set;
        }
        public HomePageViewModel()
        {
            _model = SessionManagerFactory.GetClientSessionManager();
            _model.SubscribeSession(this);
            users = new List<UserViewData>();
            userid = ChatViewModel.UserId;
        }

        public void OnClientSessionChanged(SessionData session)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<SessionData>((session) =>
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
                                    usernew.shortname = (user.username.Substring(0,2)).ToUpper();
                                    users.Add(usernew);
                                }
                                OnPropertyChanged("ListChanged");
                            }
                        }),
                        session);
        }

        public void LeftClient()
        {
            _model.RemoveClient();
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler UsersListChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void OnPropertyChanged(string property)
        {
            UsersListChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        private IUXClientSessionManager _model;
    }
}
