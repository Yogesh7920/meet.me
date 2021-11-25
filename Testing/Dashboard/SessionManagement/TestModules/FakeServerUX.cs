using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;
using Dashboard.Server.SessionManagement;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeServerUX 
    {
        public FakeServerUX(IUXServerSessionManager sessionManager)
        {
            meetingEnded = false;
            sessionManager.MeetingEnded += () => OnMeetingEnded();
        }

        public void OnMeetingEnded()
        {
            meetingEnded = true;
        }

        public bool meetingEnded;
    }
}
