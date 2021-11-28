/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake UX which subscribes and listens for the client session data as well as summary/analytics.
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeClientUX : IClientSessionNotifications
    {
        public FakeClientUX(IUXClientSessionManager sessionManager)
        {
            meetingEnded = false;
            sessionManager.MeetingEnded += () => OnMeetingEnded();
            sessionManager.AnalyticsCreated += (sessionAnalytics) => OnAnalyticsChanged(sessionAnalytics);
            sessionManager.SummaryCreated += (summary) => OnSummaryCreated(summary);
            sessionManager.SubscribeSession(this);
            sessionData = null;
        }

        public void OnAnalyticsChanged(SessionAnalytics analytics)
        {
            sessionAnalytics = analytics;
        }

        public void OnClientSessionChanged(SessionData session)
        {
            sessionData = new();
            sessionData = session;
        }

        public void OnMeetingEnded()
        {
            meetingEnded = true;
        }

        public void OnSummaryCreated(string summary)
        {
            sessionSummary = summary;
        }

        public bool meetingEnded;
        public string sessionSummary;
        public SessionData sessionData;
        public SessionAnalytics sessionAnalytics;
    }
}
