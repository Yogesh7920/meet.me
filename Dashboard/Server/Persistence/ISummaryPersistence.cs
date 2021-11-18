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
                 /// saves the summary of the session into a summary file
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return true if succesfully saved else return false </returns>
        public bool saveSummary(string message);
    }

}
