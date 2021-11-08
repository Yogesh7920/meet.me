using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class SummaryData : IRecievedFromServer
    {
        public SummaryData(string chatSummary)
        {
            summary = chatSummary;
        }
        public string summary;
    }
}
