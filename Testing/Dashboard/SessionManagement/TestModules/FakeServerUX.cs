/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake server UX which subscribes and listens for the client session data.
/// </summary>

using Dashboard.Server.SessionManagement;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeServerUX
    {
        public bool meetingEnded;

        public FakeServerUX(IUXServerSessionManager sessionManager)
        {
            meetingEnded = false;
            sessionManager.MeetingEnded += () => OnMeetingEnded();
        }

        public void OnMeetingEnded()
        {
            meetingEnded = true;
        }
    }
}