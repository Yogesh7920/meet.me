using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Persistence
{
    /// <summary>
    /// saves the summary of the session into a summary file
    /// </summary>
    /// <param name="sessionId"> takes sessionId to denote the name of the file to which we need to save. </param>
    /// <param name="message"> takes message string that need to be saved </param>
    /// <returns> return true if succesfully saved else return false </returns>
    public class SummaryPersistence : ISummaryPersistence
    {
        public override bool saveSummary(String meetingId, string message)
        {

        }
    }
}