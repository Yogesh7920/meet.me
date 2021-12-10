/// <author>P S Harikrishnan</author>
/// <created>13/11/2021</created>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Dashboard;
using Dashboard.Client.SessionManagement;


namespace Client.ViewModel
{
    public class HomePageViewModel : IClientSessionNotifications // Notifies change in list of users.
    {
        /// <summary>
        ///     Underlying data model.
        /// </summary>
        private readonly IUXClientSessionManager _model;
        private DashboardSessionData _dashboardSessionData;

        public HomePageViewModel()
        {
            _model = SessionManagerFactory.GetClientSessionManager();
            _model.SubscribeSession(this);
            users = new List<UserViewData>();
            client = new UserData();
            client.userID = -1;
            _dashboardSessionData = DashboardSessionData.GetInstance();
        }

        /// <summary>
        ///     Constructor for testing
        /// </summary>
        public HomePageViewModel(IUXClientSessionManager model)
        {
            _model = model;
            _model.SubscribeSession(this);
            users = new List<UserViewData>();
            client = new UserData();
            client.userID = -1;
        }

        public UserData client // current client's data
        {
            get;
        }

        public List<UserViewData> users { get; }

        /// <summary>
        ///     Gets the dispatcher to the main thread. In case it is not available
        ///     (such as during unit testing) the dispatcher associated with the
        ///     current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     Taking new session object when users list changes
        /// </summary>
        /// <param name="session">New session object.</param>
        public void OnClientSessionChanged(SessionData session)
        {
            if (client.userID == -1) client.userID = _model.GetUser().userID;
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<SessionData>(session =>
                {
                    lock (this)
                    {
                        users.Clear();
                        if (session != null)
                            foreach (var user in session.users)
                                if (user != null)
                                {
                                    var usernew = new UserViewData();
                                    usernew.username = user.username;
                                    if (user.userID == client.userID) usernew.username += " (You)";
                                    if (user.username.Length > 1)
                                        usernew.shortname = user.username.Substring(0, 2).ToUpper();
                                    else
                                        usernew.shortname = user.username.Substring(0, 1).ToUpper();
                                    users.Add(usernew);
                                }

                        OnPropertyChanged("ListChanged");
                    }
                }),
                session);
        }

        /// <summary>
        ///     When a client leaves
        /// </summary>
        public void LeftClient()
        {
            _model.RemoveClient();
        }

        /// <summary>
        ///     Property changed event raised when userlist gets changed.
        /// </summary>
        public event PropertyChangedEventHandler UsersListChanged;

        /// <summary>
        ///     Handles the property changed event raised.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            UsersListChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}