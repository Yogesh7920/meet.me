/// <author>Parmanand Kumar</author>
/// <created>20/11/2021</created>
/// <summary>
/// It contains Unitests for the Persistence Module 
/// </summary> 
/// 

using Dashboard.Server.Persistence;
using Dashboard.Server.Telemetry;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NUnitTest_Persistence
{
    public class Tests
    {
        /// <summary>
        /// It is just a utility function to compare to Objects of 
        /// ServerDataToSave class
        /// </summary> 
        public bool IsEqual(ServerDataToSave sdts1, ServerDataToSave sdts2)
        {
            if (sdts1.SessionCount == sdts2.SessionCount)
            {
                List<SessionSummary> lst1 = sdts1.AllSessionsSummary;
                List<SessionSummary> lst2 = sdts2.AllSessionsSummary;
                for (int i = 0; i < lst1.Count; i++)
                {
                    // Comparing each fields
                    bool t1 = lst1[i].ChatCount == lst2[i].ChatCount;
                    bool t2 = lst1[i].Score == lst2[i].Score;
                    bool t3 = lst1[i].SessionStartTime == lst2[i].SessionStartTime;
                    bool t4 = lst1[i].UserCount == lst2[i].UserCount;

                    if ((t1 && t2 && t3 && t4) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// It tests the SaveSummary function of SummmaryPersistence
        /// </summary> 
        [Test]
        public void SaveSummary_AbleToSaveFileCorrectly()
        {
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            string summary = "NUnit Testing";
            string TextToBeSaved = "Summary : --------- " + Environment.NewLine + summary + Environment.NewLine;
            // Saving a Test summary string
            ResponseEntity response = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary);

            // Reading the file if actually saved
            string TextActuallySaved = File.ReadAllText(Path.Combine(path, response.FileName));

            //If text saved and to be saved actually matches
            if (TextToBeSaved == TextActuallySaved)
            {
                Trace.WriteLine(TextToBeSaved);
                Trace.WriteLine(TextActuallySaved);
                Assert.IsTrue(response.IsSaved);
            }

        }


        /// <summary>
        /// It tests the SaveServerData function of TelemetryPersistence
        /// </summary> 
        [Test]
        public void SaveServerData_CreatesTheXmlFile()
        {
            //Creating a sample SessionSummmary instance
            SessionSummary sessionSummary = new SessionSummary();
            sessionSummary.ChatCount = 5;
            sessionSummary.Score = 10;
            sessionSummary.SessionStartTime = new DateTime(2010, 1, 1, 8, 0, 15);
            sessionSummary.UserCount = 15;

            List<SessionSummary> lst = new List<SessionSummary>();
            for (int i = 0; i < 2; i++)
            {
                lst.Add(sessionSummary);
            }

            ServerDataToSave sdtns = new ServerDataToSave();
            sdtns.SessionCount = lst.Count;
            sdtns.AllSessionsSummary = lst;

            // Saving thr Serverdata
            ResponseEntity response = PersistenceFactory.GetTelemetryPersistenceInstance().SaveServerData(sdtns);

            string path = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";

            //Checking if File actually exists
            Assert.IsTrue(File.Exists(Path.Combine(path, response.FileName)));

            //Priting the xml doc
            string TextActuallySaved = File.ReadAllText(Path.Combine(path, response.FileName));
            Trace.WriteLine(TextActuallySaved);

        }

        /// <summary>
        /// It tests the RetriveAllSeverData function of TelemetryPersistence
        /// </summary>
        [Test]
        public void RetriveAllSeverData_ReturnsServerDataCorrectly()
        {
            //Saving an Test ServerData
            SessionSummary sessionSummary = new SessionSummary();
            sessionSummary.ChatCount = 5;
            sessionSummary.Score = 10;
            sessionSummary.SessionStartTime = new DateTime(2010, 1, 1, 8, 0, 15);
            sessionSummary.UserCount = 15;

            List<SessionSummary> lst = new List<SessionSummary>();
            for (int i = 0; i < 2; i++)
            {
                lst.Add(sessionSummary);
            }

            ServerDataToSave sdtns = new ServerDataToSave();
            sdtns.SessionCount = lst.Count;
            sdtns.AllSessionsSummary = lst;

            // Saving the Test ServerData
            ResponseEntity response = PersistenceFactory.GetTelemetryPersistenceInstance().SaveServerData(sdtns);

            // Retrive and check if it matches if actually being saved
            ServerDataToSave deserialised_ouput = PersistenceFactory.GetTelemetryPersistenceInstance().RetrieveAllSeverData();
            Trace.WriteLine(deserialised_ouput.SessionCount + " " + deserialised_ouput.AllSessionsSummary[0].SessionStartTime);

            // Calling IsEqual to verify if both ServerData to be saved and retrived Server Data Matches
            Assert.IsTrue(IsEqual(sdtns, deserialised_ouput));

        }

        /// <summary>
        /// It tests the Save function of TelemetryPersistence
        /// </summary>
        [Test]
        public void Save__SavesAllAnalytics()
        {
            //Creating Test Instance of All Analytics data
            TelemetryPersistence tp = new TelemetryPersistence();
            Dictionary<int, int> UserCountVsTimeStamp = new Dictionary<int, int>();
            Dictionary<int, int> chatCountVsUserId = new Dictionary<int, int>();
            List<int> insincereList = new List<int>();
            insincereList.Add(1);
            insincereList.Add(2);
            insincereList.Add(3);
            insincereList.Add(4);

            UserCountVsTimeStamp.Add(1131, 12);
            UserCountVsTimeStamp.Add(1124, 15);
            UserCountVsTimeStamp.Add(1125, 18);

            chatCountVsUserId = UserCountVsTimeStamp;
            SessionAnalytics sessionAnalytics = new SessionAnalytics();
            sessionAnalytics.ChatCountForEachUser = chatCountVsUserId;
            sessionAnalytics.UserCountAtAnyTime = UserCountVsTimeStamp;
            sessionAnalytics.InsincereMembers = insincereList;

            //Actually Saving it
            ResponseEntity response = tp.Save(sessionAnalytics);

            string p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + response.FileName;

            // Check if such .png and .txt files are present or not
            bool IsChatCountForUserSaved = File.Exists(Path.Combine(p1, "ChatCountVsUserID.png"));
            bool IsInsincereMembersSaved = File.Exists(Path.Combine(p1, "insincereMembersList.txt"));
            bool IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            Assert.IsTrue(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
        }
    }
}