using Dashboard;
using Dashboard.Client.SessionManagement;
using System;

namespace Testing.Dashboard.TestModels
{
    class TestUX : IClientSessionNotifications
    {
        public TestUX(IUXClientSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            gotNotified = false;
            _sessionManager.SummaryCreated += (summary) => UpdateSummary(summary);
            _sessionManager.MeetingEnded += () => OnMeetingEnds();
            summary = null;
            meetingEndEvent = false;
        }
        public void OnClientSessionChanged(SessionData session)
        {
            sessionData = session;
            Console.WriteLine(session);
            gotNotified = true;
        }

        private void UpdateSummary(string recievedSummary)
        {
            summary = recievedSummary;
        }

        private void OnMeetingEnds()
        {
            meetingEndEvent = true;
        }

        public string summary;
        public bool gotNotified;
        public bool meetingEndEvent;
        private IUXClientSessionManager _sessionManager;
        public SessionData sessionData;
    }
}
