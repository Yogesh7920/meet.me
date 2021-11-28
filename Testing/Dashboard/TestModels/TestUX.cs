/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the test UX
///		for testing purpose
/// </summary>

using System;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;

namespace Testing.Dashboard.TestModels
{
    internal class TestUX : IClientSessionNotifications
    {
        private readonly IUXClientSessionManager _sessionManager;
        public bool gotNotified;
        public bool meetingEndEvent;
        public SessionAnalytics sessionAnalytics;
        public SessionData sessionData;

        public string summary;

        public TestUX(IUXClientSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            gotNotified = false;
            _sessionManager.SummaryCreated += summary => UpdateSummary(summary);
            _sessionManager.MeetingEnded += () => OnMeetingEnds();
            _sessionManager.AnalyticsCreated += sessionAnalytics => UpdateAnalytics(sessionAnalytics);
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
    }
}