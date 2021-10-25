using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Persistence
{
    public interface ISummaryPersistence
    {
        bool saveSummary(String meetingId, string message);
    }

    public class SummaryPersistence : ISummaryPersistence
    {
        public override bool saveSummary(String meetingId, string message)
        {

        }
    }
}
