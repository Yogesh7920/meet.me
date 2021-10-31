using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.SessionManagement
{
    public interface ITelemetryNotifications
    {
        /// <summary>
        /// Handles the change in the Global session (SessionData Object)
        /// </summary>
        /// <param name="newSession"> The changed session </param>
        void OnAnalyticsChanged(SessionData newSession);
    }
}

