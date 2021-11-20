/// <author>Parmanand Kumar</author>
/// <created>15/11/2021</created>
/// <summary>
///     It contains the SummaryPersistence class
///     It implements the ISummaryPersistence interface functions.
/// </summary> 

using System;
using System.Diagnostics;
using System.IO;

namespace Dashboard.Server.Persistence
{

    public class SummaryPersistence : ISummaryPersistence
    {
        /// <summary>
        ///     saves the summary of the session into a summary file
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return true if succesfully saved else return false </returns>
        public ResponseEntity SaveSummary(string message, bool testMode)
        {
            // Creating the name of the File, according to Current DateTime in feasible format of file. 
            string sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            // Storing the Path
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";

            // Summary Creation
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;

            ResponseEntity response = new ResponseEntity();
            response.FileName = sessionId1 + ".txt";

            //Check if Such folder exists if not create Folder for the same
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //Writing to the Text file
            File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
            Trace.WriteLine("Summary saved Suceessfully!!");
            response.IsSaved = true;
            return response;

        }

        /// <summary>
        ///     saves the summary of the session into a summary file
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return true if succesfully saved else return false </returns>
        public bool SaveSummary(string message)
        {
            // Creating the name of the File, according to Current DateTime in feasible format of file. 
            string sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            // Storing the Path
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";

            // Summary Creation
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;

            ResponseEntity response = new ResponseEntity();
            response.FileName = sessionId1 + ".txt";

            //Check if Such folder exists if not create Folder for the same
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //Writing to the Text file
            File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
            Trace.WriteLine("Summary saved Suceessfully!!");
            response.IsSaved = true;
            return response.IsSaved;

        }

    }


}
