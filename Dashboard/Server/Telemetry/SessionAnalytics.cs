using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Telemetry
{
    ///<summary>
    /// The data used to plot the visuals
    ///</summary>
    public class SessionAnalytics{

        public SessionAnalytics()
        {

        }
        
        /// Stores the userCount(int) at every time stamp(DateTime)
        public Dictionary<DateTime, int> userCountAtAnyTime;

        /// Stores the chat count(int) for each user(UserData) 
        public Dictionary<int, int> chatCountForEachUser;
        
        /// returns the users who were present in the session for less than
        /// a certain minimum time(threshold time)
        public List<int> insincereMembers;
    }
}
