using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.SessionManagement;
using Content;

namespace Dashboard.Server.Telemetry
{
    public interface ITelemetry
    {
        /// <summary>
        ///    SM uses it to notify the session is over, and the analytics need to be store
        /// </summary>

        /// <params name="AllMessages">The chat data of all the threads</params>
        void SaveAllAnalytics(List<Content.models.Threads> AllMessages);

        /// <summary>
        ///    SM uses it to get the analysed data to pass on to the UI
        /// </summary>
        
        /// <returns> TelemetryAnalyticsModel object </returns>
        TelemetryAnalyticsModel getTelemetryAnalytics();
    }    
    }
}
