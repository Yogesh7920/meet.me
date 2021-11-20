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
        public bool IsEqual(ServerDataToSave sdts1, ServerDataToSave sdts2)
        {
            if (sdts1.SessionCount == sdts2.SessionCount)
            {
                List<SessionSummary> lst1 = sdts1.AllSessionsSummary;
                List<SessionSummary> lst2 = sdts2.AllSessionsSummary;
                for (int i = 0; i < lst1.Count; i++)
                {
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

        [Test]
        public void SaveSummary_AbleToSaveFileCorrectly()
        {
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            string summary = "NUnit Testing";
            string TextToBeSaved = "Summary : --------- " + Environment.NewLine + summary + Environment.NewLine;
            ResponseEntity response = PersistenceFactory.GetSummaryPersistenceInstance().SaveSummary(summary);

            string TextActuallySaved = File.ReadAllText(Path.Combine(path, response.FileName));

            if (TextToBeSaved == TextActuallySaved)
            {
                Trace.WriteLine(TextToBeSaved);
                Trace.WriteLine(TextActuallySaved);
                Assert.IsTrue(response.IsSaved);
            }

        }

        [Test]
        public void SaveServerData_CreatesTheXmlFile()
        {
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

            ResponseEntity response = PersistenceFactory.GetTelemetryPersistenceInstance().SaveServerData(sdtns);

            string path = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";

            Assert.IsTrue(File.Exists(Path.Combine(path, response.FileName)));

            string TextActuallySaved = File.ReadAllText(Path.Combine(path, response.FileName));
            Trace.WriteLine(TextActuallySaved);

        }

        [Test]
        public void RetriveAllSeverData_ReturnsServerDataCorrectly()
        {
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

            ResponseEntity response = PersistenceFactory.GetTelemetryPersistenceInstance().SaveServerData(sdtns);

            ServerDataToSave deserialised_ouput = PersistenceFactory.GetTelemetryPersistenceInstance().RetrieveAllSeverData();
            Trace.WriteLine(deserialised_ouput.SessionCount + " " + deserialised_ouput.AllSessionsSummary[0].SessionStartTime);

            Assert.IsTrue(IsEqual(sdtns, deserialised_ouput));

        }

        [Test]
        public void Save__SavesAllAnalytics()
        {
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
            ResponseEntity response = tp.Save(sessionAnalytics);

            string p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + response.FileName;
            bool IsChatCountForUserSaved = File.Exists(Path.Combine(p1, "ChatCountVsUserID.png"));
            bool IsInsincereMembersSaved = File.Exists(Path.Combine(p1, "insincereMembersList.txt"));
            bool IsUserCountAtAnyTimeSaved = File.Exists(Path.Combine(p1, "UserCountVsTimeStamp.png"));
            Assert.IsTrue(IsChatCountForUserSaved && IsInsincereMembersSaved && IsUserCountAtAnyTimeSaved);
        }
    }
}