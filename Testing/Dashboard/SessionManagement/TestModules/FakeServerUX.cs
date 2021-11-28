/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake server UX which subscribes and listens for the client session data.
/// </summary>

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
