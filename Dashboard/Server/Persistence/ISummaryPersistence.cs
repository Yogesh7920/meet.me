using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Persistence
{
    //SummaryPersistence Interface
    public interface ISummaryPersistence
    {
          /// <summary>
                 /// append the summary of the session into a summary file
        /// </summary>
        /// <params> takes sessionId and message string
        bool saveSummary(String sessionId, string message);
    }

    public class SummaryPersistence : ISummaryPersistence
    {
        public override bool saveSummary(String meetingId, string message)
        {
            
        }
    }
}
