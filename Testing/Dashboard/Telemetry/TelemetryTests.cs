using Dashboard;
using Dashboard.Server.Persistence;
using Dashboard.Server.Telemetry;
using Content;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Testing.Dashboard.Telemetry
{
    public class TelemetryNunitTests
    {
        [Test]
        public void OnAnalyticsChanged_GetUserCountVsTimeStamp_UpdatingUserCount()
        {
            // Arrange
            UserData user1= new UserData("Noah",1);
            SessionData session1= new SessionData();
            session1.AddUser(user1);
            UserData user2= new UserData("Shailee",2);
            SessionData session2=new SessionData();
            session2.AddUser(user1);
            session2.AddUser(user2);
            //Act
            TelemetryFactory.GetTelemetryInstance().OnAnalyticsChanged(session1);
            int userCount1 = TelemetryFactory.GetTelemetryInstance().userCountAtEachTimeStamp[DateTime.Now];
            bool check1=false;
            // total users in session1 is 1
            if(userCount1==1) check1=true;
            TelemetryFactory.GetTelemetryInstance().OnAnalyticsChanged(session2);
            int userCount2= TelemetryFactory.GetTelemetryInstance().userCountAtEachTimeStamp[DateTime.Now];
            bool check2=false;
            // total users in session2 is 2.
            if(userCount2==2) check2=true;
            //Assert
            Assert.isTrue(check1 && check2);
            
        }

        [Test]
        public void OnAnalyticsChanged_GetInsincereMembers_UpdatingInsincereMembers()
        {
            // Arrange
            UserData user1= new UserData("Noah",1);
            SessionData session1= new SessionData();
            session1.AddUser(user1);
            UserData user2= new UserData("Shailee",2);
            SessionData session2=new SessionData();
            session2.AddUser(user2);
            // user1 left the meeting in session2
            // Act
            TelemetryFactory.GetTelemetryInstance().OnAnalyticsChanged(session1);
            TelemetryFactory.GetTelemetryInstance().OnAnalyticsChanged(session2);
            int elementAtZero = TelemetryFactory.GetTelemetryInstance().insincereMembers[0];
            //Assert
            Assert.AreEqual(elementAtZero,1);
        }

        [Test]
        public void SaveAnalytics_GetUserVsChatCount_CorrectChatCountForAnyUser()
        {
            // Arrange
            ReceiveMessageData  message1= new ReceiveMessageData();
            message1.Message= "Hello from user 1";
            message1.SenderId=1;
            ReceiveMessageData  message2= new ReceiveMessageData();
            message1.Message= "Another Hello from user 1";
            message1.SenderId=1;
            ReceiveMessageData  message3= new ReceiveMessageData();
            message1.Message= "Hello from user 2";
            message1.SenderId=2;

            List<ReceiveMessageData> msgList1 = new List<ReceiveMessageData>();
            msgList1.Add(message1);
            msgList1.Add(message2);
            List<ReceiveMessageData> msgList2 = new List<ReceiveMessageData>();
            msgList2.Add(message3);

            ChatContext[] allMessages = new ChatContext[2];
            ChatContext chat1 = new ChatContext();
            chat1.CreationTime = DateTime.Now;
            chat1.MsgList=msgList1;
            ChatContext chat2 = new ChatContext();
            chat2.CreationTime = DateTime.Now;
            chat2.MsgList=msgList2;
            allMessages[0]=chat1;
            allMessages[1]=chat2;

            //Act
            TelemetryFactory.GetTelemetryInstance().SaveAnalytics(allMessages);
            int chatCountUser1= TelemetryFactory.GetTelemetryInstance().userIdChatCountDic[1];
            bool check1=false;
            if(chatCountUser1==2)
            {
                check1=true;
            }
            int chatCountUser2=TelemetryFactory.GetTelemetryInstance(). userIdChatCountDic[2];
            bool check2=false;
            if(chatCountUser2==1)
            {
                check2=true;
            }
            
            //Assert
            Assert.isTrue(check1 && check2);

        }

        [Test]
        public void UpdateServerData_Retreiving_RetrievingCorrectly()
        {
            // Arrange
            ServerDataToSave serverData=_persistence. RetrieveAllSeverData();
            Assert.IsNotNull(serverData);
        }

        [Test]
        public void UpdateServerData_ServerData_UpdatingCorrectly()
        {
            //Arrange
            ServerDataToSave serverData = new ServerDataToSave();
            serverData.sessionCount=0;
            serverData.allSessionsSummary=new List<SessionSummary>();
            SessionSummary session1 = new SessionSummary();
            session1.chatCount=100;
            session1.userCount=30;
            session1.score=3000;
            //Act
            serverData.sessionCount++;
            serverData.allSessionsSummary.Add(session1);
            //Assert
            Assert.IsTrue(serverData.allSessionsSummary.Count==1);
            
        }

        [Test]
        public void UpdateServerData_SaveServerData_SavingCorrectly()
        {
            //Arrange
            ServerDataToSave serverData = new ServerDataToSave();
            serverData.sessionCount=0;
            serverData.allSessionsSummary=new List<SessionSummary>();
            SessionSummary session1 = new SessionSummary();
            session1.chatCount=100;
            session1.userCount=30;
            session1.score=3000;
            serverData.sessionCount++;
            serverData.allSessionsSummary.Add(session1);
            //Act
            ResponseEntity response = _persistence.SaveServerData(serverData);
            Assert.IsTrue(response.IsSaved);
        }
        private readonly ITelemetryPersistence _persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
    }
}
