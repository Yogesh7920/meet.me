using System.Collections.Generic;

namespace Dashboard
{
    public class ServerDataToSave
    {

        /// Dictionary with SessionData as key and its score(ChatCount * No. of users) as value

        Dictionary<int, int> ScoreOfEachSession;

        /// Dictionary with SessionData as key and  total user count as value
        Dictionary<int, int> UserCountForEachSession;

        /// Dictionary with SessionData as key and total chat count as value
        Dictionary<int, int> ChatCountForEachSession;
    }
}

