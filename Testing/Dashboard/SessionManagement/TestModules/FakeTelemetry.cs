/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake telemetr which subscribes and listens for the client session data and provide mock analytics.
/// </summary>

using Content;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Dashboard.Server.Telemetry;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeTelemetry : ITelemetry, ITelemetryNotifications
    {
        public bool analyticsChanged;
        public SessionAnalytics sessionAnalytics;

        public FakeTelemetry(ITelemetrySessionManager sessionManager)
        {
            sessionManager.Subscribe(this);
            analyticsChanged = false;
            sessionAnalytics = new SessionAnalytics();
        }

        public void SaveAnalytics(ChatContext[] allMessages)
        {
        }

        public SessionAnalytics GetTelemetryAnalytics(ChatContext[] allMessages)
        {
            return sessionAnalytics;
        }

        public void OnAnalyticsChanged(SessionData newSession)
        {
            analyticsChanged = true;
        }
    }
}