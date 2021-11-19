using System.Collections.Generic;

namespace Dashboard
{
    public class TelemetryAnalyticsModel
    {
        /// Stores the chat count(int) for each user(UserData)
        private Dictionary<int, int> ChatCountForEachUser;

        /// returns the users who were present in the session for less than the minimum time(threshold time)
        private List<int> InsincereMembers;

        /// Stores the userCount(int) at every time stamp(DateTime)
        private Dictionary<int, int> UserCountAtAnyTime;
    }
}