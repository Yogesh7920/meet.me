using Dashboard;
using Dashboard.Client.SessionManagement;
using System;

namespace Testing.Dashboard.TestModels
{
    class TestUX : IClientSessionNotifications
    {
        public TestUX()
        {
            gotNotified = false;
        }
        public void OnClientSessionChanged(SessionData session)
        {
            sessionData = session;
            Console.WriteLine(session);
            gotNotified = true;
        }

        public bool gotNotified;
        public SessionData sessionData;
    }
}
