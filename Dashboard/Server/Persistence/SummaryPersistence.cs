using System;
using System.Diagnostics;
using System.IO;

namespace Dashboard.Server.Persistence
{

    public class SummaryPersistence : ISummaryPersistence
    {
        public ResponseEntity SaveSummary(string message)
        {
            string sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;
            ResponseEntity response = new ResponseEntity();
            response.FileName = sessionId1 + ".txt";
            
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
            Trace.WriteLine("Summary saved Suceessfully!!");
            response.IsSaved = true;
            return response;
            
        }
    }


}
