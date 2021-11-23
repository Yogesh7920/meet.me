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


        public SummaryPersistence()
        {
            path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
        }
        /// <summary>
        ///     This function is created for the purpose of testing, by returnning
        ///     the file name of the saved function to check if correctly saved or not.
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return ResponseEntity(IsSaved as true and FileName of the same) if succesfully saved else return IsSaved as false in ResponseEntity </returns>
        public ResponseEntity SaveSummary(string message, bool testMode)
        {
            // Creating the name of the File, according to Current DateTime in feasible format of file. 
            string sessionId1 = string.Format("Summary_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);
            // Storing the Path
           // string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";

            // Summary Creation
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;

            ResponseEntity response = new ResponseEntity();
            response.FileName = sessionId1 + ".txt";
            try
            {
                //Check if Such folder exists if not create Folder for the same
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                //Writing to the Text file
                File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
                Trace.WriteLine("Summary saved Suceessfully!!");
                response.IsSaved = true;
                return response;
            }
            catch(Exception except)
            {
                Trace.WriteLine(except.Message);
                response.IsSaved = false;
                return response;
            }
            
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
           // string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";

            // Summary Creation
            string createText = "Summary : --------- " + Environment.NewLine + message + Environment.NewLine;

            ResponseEntity response = new ResponseEntity();
            response.FileName = sessionId1 + ".txt";

            try
            {
                //Check if Such folder exists if not create Folder for the same
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                //Writing to the Text file
                File.WriteAllText(Path.Combine(path, sessionId1 + ".txt"), createText);
                Trace.WriteLine("Summary saved Suceessfully!!");
                response.IsSaved = true;
                return response.IsSaved;
            }
            catch(Exception except)
            {
                Trace.WriteLine(except.Message);
                response.IsSaved = false;
                return response.IsSaved;
            }

        }

        private string path;

        public string summaryPath { get => path; set => path = value; }
    }


}
