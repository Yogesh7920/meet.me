using System;
using System.Diagnostics;
using System.IO;

namespace Dashboard.Server.Persistence
{

    public class SummaryPersistence : ISummaryPersistence
    {
        public bool saveSummary(string message)
        {
            string sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
                Trace.WriteLine("Summary saved Suceessfully!!");
                return true;
            }
            catch (IOException exp)
            {
                Trace.WriteLine(exp.Message);
                return false;
            }
        }
    }


}
