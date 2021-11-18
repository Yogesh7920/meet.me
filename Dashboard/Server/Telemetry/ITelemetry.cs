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
        ///    simiplifies all_messages into easily plotable data.
        /// </summary>
        /// <params name="allMessages"> Array of ChatContext object, which contains info about threads </params>
        void SaveAnalytics(ChatContext[] allMessages);

        /// <summary>
        ///    Returns the analysed data to be passed to the UI
        /// </summary>
        /// <params name="allMessages"> The chat data of all the threads </params>
        /// <returns> SessionAnalytics object </returns>
        SessionAnalytics GetTelemetryAnalytics(ChatContext[] allMessages);
        
    }
}
