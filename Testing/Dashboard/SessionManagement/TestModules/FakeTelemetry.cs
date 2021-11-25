using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Dashboard.Server.Telemetry;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeTelemetry : ITelemetry, ITelemetryNotifications
    {
        public FakeTelemetry(ITelemetrySessionManager sessionManager)
        {
            sessionManager.Subscribe(this);
            analyticsChanged = false;
            sessionAnalytics = new();
        }
        public void OnAnalyticsChanged(SessionData newSession)
        {
            analyticsChanged = true;
        }

        public void SaveAnalytics(ChatContext[] allMessages)
        {
            
        }

        public SessionAnalytics GetTelemetryAnalytics(ChatContext[] allMessages)
        {
            return sessionAnalytics;
        }

        public bool analyticsChanged;
        public SessionAnalytics sessionAnalytics;
    }
}
