using Dashboard;
using Dashboard.Client.SessionManagement;
using System.Collections.Generic;
using System.Diagnostics;

namespace Client.ViewModel
{
    /// <summary>
    /// Singleton class to share session data with DashboardViewModel
    /// </summary>
    public sealed class DashboardSessionData : IClientSessionNotifications
    {
        private DashboardSessionData()
        {
            _clientSM = SessionManagerFactory.GetClientSessionManager();
            _clientSM.SubscribeSession(this);
            _latestSessionData = new();
            _allUsers = new();
            Trace.WriteLine("[UX] Dashboard Subscribed to Session Notifications");
        }

        private static DashboardSessionData _instance;

        public static DashboardSessionData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DashboardSessionData();
            }
            return _instance;
        }

        public SessionData UpdateSessionData(SessionData dataToUpdate)
        {
            lock (this)
            {
                dataToUpdate = _latestSessionData;
                return dataToUpdate;
            }
        }

        public List<UserData> GetAllUsers()
        {
            return _allUsers;
        }

        public static SessionData GetSessionData()
        {
            return _latestSessionData;
        }

        public void OnClientSessionChanged(SessionData session)
        {
            if (session != null)
            {
                _latestSessionData = session;
                foreach (UserData user in session.users)
                {
                    if (!_allUsers.Contains(user))
                        _allUsers.Add(user);
                }
            }
        }

        private static SessionData _latestSessionData;
        private static List<UserData> _allUsers;
        private IUXClientSessionManager _clientSM;
    }
}