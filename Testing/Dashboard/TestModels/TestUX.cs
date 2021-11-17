using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;
using Dashboard.Client.SessionManagement;

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
