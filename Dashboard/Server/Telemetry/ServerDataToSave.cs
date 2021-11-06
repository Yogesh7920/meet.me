
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class ServerDataToSave
    {
      
        /// Dictionary with SessionData as key and its score(ChatCount * No. of users) as value
        
        Dictionary<SessionData, int> ScoreOfEachSession;

        /// Dictionary with SessionData as key and  total user count as value
        Dictionary<SessionData, int> UserCountForEachSession;

        /// Dictionary with SessionData as key and total chat count as value
        Dictionary<SessionData, int> ChatCountForEachSession;
    }
}

