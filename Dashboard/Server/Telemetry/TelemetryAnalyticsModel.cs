using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class TelemetryAnalyticsModel{
        /// Stores the userCount(int) at every time stamp(DateTime)
        Dictionary<DateTime, int> UserCountAtAnyTime;
        /// Stores the chat count(int) for each user(UserData) 
        Dictionary<UserData, int> ChatCountForEachUser;
        /// returns the users who were present in the session for less than the minimum time(threshold time)
        List<UserData> InsincereMembers;
    }
}
