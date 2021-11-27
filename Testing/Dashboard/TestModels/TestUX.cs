using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;

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
            _sessionManager.AnalyticsCreated += (sessionAnalytics) => UpdateAnalytics(sessionAnalytics);
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

        private void UpdateAnalytics(SessionAnalytics sessionAnalytics)
        {
            this.sessionAnalytics = sessionAnalytics;
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
        public SessionAnalytics sessionAnalytics;
    }
}
