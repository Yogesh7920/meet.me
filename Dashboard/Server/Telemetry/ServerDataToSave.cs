
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    /// <summary>
    /// The server data that would be stored in Persistance
    /// </summary>
    public class ServerDataToSave
    {
        ///<summary>
        ///     the total number of sessions
        ///</summary>
        public int sessionCount;

        /// <summary>
        ///     List of SessionSummary that is summary of each session
        /// </summary>
        public List<SessionSummary> allSessionsSummary;
    }
}

