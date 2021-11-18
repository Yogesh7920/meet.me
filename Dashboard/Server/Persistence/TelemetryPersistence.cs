//Author: Parmanand Kumar
using Dashboard.Server.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace Dashboard.Server.Persistence
{

    public class TelemetryPersistence : ITelemetryPersistence
    {
        public ServerDataToSave retriveAllSeverData()
        {
            XmlSerializer deserialiser = new XmlSerializer(typeof(ServerDataToSave));
            string path = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";
            object objectsList = null;
            try
            {
                using (StreamReader stream = new StreamReader(Path.Combine(path, "GlobalServerData.xml")))
                {
                    objectsList = deserialiser.Deserialize(stream);
                }
                return (ServerDataToSave)objectsList;
            }
            catch (IOException exp)
            {
                Trace.WriteLine(exp.Message);
                return (ServerDataToSave)objectsList;
            }
        }

        public bool save(SessionAnalytics sessionAnalyticsData)
        {
            // create folder of name sessionId to store all analytics data
            
            string sessionId = string.Format("Analytics_{0:yyyy - MM - dd_hh - mm - ss - tt}", DateTime.Now);

            // Logic to plot and save UserCount Vs TimeStamp

            bool t1 = UserCountVsTimeStamp_PlotUtil(sessionAnalyticsData.UserCountAtAnyTime, sessionId);

            // Logic to plot and save ChatCount Vs UserID

            bool t2 = ChatCountVsUserID_PlotUtil(sessionAnalyticsData.ChatCountForEachUser, sessionId);

            // Logic to save InsincereMembers list

            bool t3 = InsincereMembers_SaveUtil(sessionAnalyticsData.InsincereMembers, sessionId);

            return t1 & t2 & t3;
        }

        public bool InsincereMembers_SaveUtil(List<int> InsincereMembers, string sessionId)
        {
            string p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + sessionId;
            string TextToSave = "Followings are UserIDs of InsincereMembers : " + Environment.NewLine;

            foreach (int w in InsincereMembers)
            {
                TextToSave = TextToSave + w.ToString() + Environment.NewLine;
            }

            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                File.WriteAllText(Path.Combine(p1, "insincereMembersList.txt"), TextToSave);
                Trace.WriteLine("insincereMembersList.txt saved Successfully!!");
            }
            catch (IOException exp)
            {
                Trace.WriteLine(exp.Message);
                return false;
            }

            return true;
        }
        public bool ChatCountVsUserID_PlotUtil(Dictionary<int, int> ChatCountForEachUser, string sessionId)
        {
            string p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + sessionId;
            int[] val1 = ChatCountForEachUser.Values.ToArray();
            double[] values1 = new double[val1.Length];
            for (int i = 0; i < val1.Length; i++)
            {
                values1[i] = val1[i];
            }
            List<double> pos1 = new List<double>();
            List<string> lb1 = new List<string>();

            int x1 = 0;
            foreach (int k1 in ChatCountForEachUser.Keys)
            {
                pos1.Add(x1);
                lb1.Add(k1.ToString());
                x1++;
            }

            string[] labels1 = lb1.ToArray();
            double[] positions1 = pos1.ToArray();
            var plt1 = new ScottPlot.Plot(600, 400);

            plt1.AddBar(values1, positions1);
            plt1.XTicks(positions1, labels1);
            plt1.SetAxisLimits(yMin: 0);
            plt1.YAxis.ManualTickSpacing(1);
            plt1.XLabel("UserID");
            plt1.YLabel("ChatCount for any User");

            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt1.SaveFig(Path.Combine(p1, "ChatCountVsUserID.png"));
                Trace.WriteLine("ChatCountVsUserID.png saved Successfully!!");
            }
            catch (IOException exp)
            {
                Trace.WriteLine(exp.Message);
                return false;
            }
            return true;
        }

        public bool UserCountVsTimeStamp_PlotUtil(Dictionary<int, int> UserCountAtAnyTime, string sessionId)
        {
            int[] val = UserCountAtAnyTime.Values.ToArray();
            double[] values = new double[val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                values[i] = val[i];
            }
            List<double> pos = new List<double>();
            List<string> lb = new List<string>();
            int x = 0;
            foreach (int k in UserCountAtAnyTime.Keys)
            {
                pos.Add(x);
                lb.Add(k.ToString());
                x++;
            }

            string[] labels = lb.ToArray();
            double[] positions = pos.ToArray();
            var plt = new ScottPlot.Plot(600, 400);

            var temp = plt.AddBar(values, positions);

            plt.XTicks(positions, labels);
            plt.YAxis.ManualTickSpacing(1);
            plt.SetAxisLimits(yMin: 0);
            temp.FillColor = Color.Green;
            plt.XLabel("TimeStamp");
            plt.YLabel("UserCount At Any Instant");
            string p1 = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/TelemetryAnalytics/" + sessionId;
            try
            {
                if (!Directory.Exists(p1)) Directory.CreateDirectory(p1);
                plt.SaveFig(Path.Combine(p1, "UserCountVsTimeStamp.png"));
                Trace.WriteLine("UserCountVsTimeStamp.png saved Successfully!!");
            }
            catch (IOException exp)
            {
                Trace.WriteLine(exp.Message);
                return false;
            }
            return true;
        }

        public bool saveServerData(ServerDataToSave AllserverData)
        {
            try
            {
                XmlSerializer xmlser = new XmlSerializer(typeof(ServerDataToSave));
                string path = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (System.IO.StreamWriter stream = new System.IO.StreamWriter(Path.Combine(path, "GlobalServerData.xml")))
                {
                    xmlser.Serialize(stream, AllserverData);
                }
                Trace.WriteLine("ServerData saved Succesfully!!");
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