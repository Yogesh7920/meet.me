/// <author>Harsh Parihar</author>
/// <created> 20/11/2021 </created>
/// <summary>
/// All required unit tests for Telemetry
/// </summary>

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
    /// <summary>
    /// Contains all Telemetry Test cases
    /// </summary>
    public class TelemetryNunitTests
    {
        /// <summary>
        /// Tests the GetUserCountVsTimeStamp function
        /// and checks if user count is updated after each session
        /// change
        /// </summary>
        [Test]
        public void GetUserCountVsTimeStamp_UpdatingUserCount_OneUserInSession1TwoInSession2()
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
            DateTime currTime= new DateTime(2021,11,23,1,0,0);
            TelemetryFactory.GetTelemetryInstance().GetUserCountVsTimeStamp(session1,currTime);
            int userCount1 = TelemetryFactory.GetTelemetryInstance().userCountAtEachTimeStamp[currTime];
            bool check1=false;
            // total users in session1 is 1
            if(userCount1==1) check1=true;
            DateTime time2= new DateTime(2021,11,23,1,30,0);
            TelemetryFactory.GetTelemetryInstance().GetUserCountVsTimeStamp(session2, time2);
            int userCount2= TelemetryFactory.GetTelemetryInstance().userCountAtEachTimeStamp[time2];
            bool check2=false;
            // total users in session2 is 2.
            if(userCount2==2) check2=true;
            //Assert
            Assert.IsTrue(check1 && check2);
            
        }

        /// <summary>
        /// Tests if EntryTime of each user is stored correctly
        /// </summary>
        [Test]
        public void CalculateEnterExitTimes_EntryTime_IsCorrect()
        {
            // Arrange
            UserData user1 = new UserData("Noah", 1);
            SessionData session1 = new SessionData();
            session1.AddUser(user1);
            DateTime time1 = new DateTime(2021, 11, 23, 1, 0, 0);
            //Act
            TelemetryFactory.GetTelemetryInstance().CalculateEnterExitTimes(session1, time1);
            //Assert
            Assert.AreEqual(TelemetryFactory.GetTelemetryInstance().userEnterTime[user1],time1);
        }

        /// <summary>
        /// Tests if ExitTime of each user is stored correctly
        /// </summary>
        [Test]
        public void CalculateEnterExitTimes_ExitTime_IsCorrect()
        {
            // Arrange
            UserData user1 = new UserData("Noah", 1);
            SessionData session1 = new SessionData();
            session1.AddUser(user1);
            DateTime time1 = new DateTime(2021, 11, 23, 1, 0, 0);
            // Act
            TelemetryFactory.GetTelemetryInstance().CalculateEnterExitTimes(session1, time1);
            // Arrange
            // In session2 user 1 left at time2
            UserData user2 = new UserData("Shailee", 2);
            SessionData session2 = new SessionData();
            session2.AddUser(user2);
            DateTime time2 = new DateTime(2021, 11, 23, 1, 15, 0);
            //Act
            TelemetryFactory.GetTelemetryInstance().CalculateEnterExitTimes(session2, time2);
            //Assert
            Assert.AreEqual(time2, TelemetryFactory.GetTelemetryInstance().userExitTime[user1]);
        }

        /// <summary>
        /// Checks if insincere members are being added in the list or not
        /// </summary>
        [Test]
        public void GetInsincereMembers_UpdatingInsincereMembers_User1IdMustBeAdded()
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
            DateTime time1= new DateTime(2021,11,23,1,0,0);
            DateTime time2= new DateTime(2021,11,23,1,15,0);
            // Getting entry time and exit time for each user
            TelemetryFactory.GetTelemetryInstance().CalculateEnterExitTimes(session1,time1);
            TelemetryFactory.GetTelemetryInstance().CalculateEnterExitTimes(session2,time2);
            TelemetryFactory.GetTelemetryInstance().GetInsincereMembers();
            int elementAtZero = TelemetryFactory.GetTelemetryInstance().insincereMembers[0];
            //Assert
            Assert.AreEqual(elementAtZero,1);
        }

        /// <summary>
        /// checks if getting correct chat count for each user
        /// </summary>
        [Test]
        public void GetUserVsChatCount_CorrectChatCountForAnyUser_TwoForUser1AndOneForUser2()
        {
            // Arrange
            // two messages for user1 and one for user2
            ReceiveMessageData  message1= new ReceiveMessageData();
            message1.Message= "Hello from user 1";
            message1.SenderId=1;
            ReceiveMessageData  message2= new ReceiveMessageData();
            message2.Message= "Another Hello from user 1";
            message2.SenderId=1;
            ReceiveMessageData  message3= new ReceiveMessageData();
            message3.Message= "Hello from user 2";
            message3.SenderId=2;

            List<ReceiveMessageData> msgList1 = new List<ReceiveMessageData>();
            msgList1.Add(message1);
            msgList1.Add(message2);
            List<ReceiveMessageData> msgList2 = new List<ReceiveMessageData>();
            msgList2.Add(message3);

            ChatContext[] allMessages = new ChatContext[2];
            ChatContext chat1 = new ChatContext();
            chat1.CreationTime = new DateTime(2021,11,23,4,0,0);
            chat1.MsgList=msgList1;
            ChatContext chat2 = new ChatContext();
            chat2.CreationTime = new DateTime(2021,11,23,4,20,0);
            chat2.MsgList=msgList2;
            allMessages[0]=chat1;
            allMessages[1]=chat2;

            //Act
            TelemetryFactory.GetTelemetryInstance().GetUserVsChatCount(allMessages);
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
            Assert.IsTrue(check1 && check2);

        }

        /// <summary>
        /// checks if serverData is getting updated correctly
        /// </summary>
        [Test]
        public void UpdateServerData_ServerData_UpdatingCorrectly()
        {
            //Arrange
            ServerDataToSave serverData = new ServerDataToSave();
            serverData.sessionCount=0;
            serverData.allSessionsSummary=new List<SessionSummary>();
            int totalUsers=30;
            int totalChats=3000;
            //Act
            TelemetryFactory.GetTelemetryInstance().UpdateServerData(serverData,totalUsers,totalChats);
            //Assert
            SessionSummary currSessionSummary = serverData.allSessionsSummary[0];
            bool correctUserCount= currSessionSummary.userCount == totalUsers;
            bool correctChatCount= currSessionSummary.chatCount == totalChats;
            bool correctScore    = currSessionSummary.score == totalChats*totalUsers;
            Assert.IsTrue(correctUserCount && correctChatCount && correctScore);
            
        }
        
        /// <summary>
        /// checks if serverData is saved correctly by persistance
        /// </summary>
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
            Directory.Delete("../../../Persistence", true);
            Assert.IsTrue(response.IsSaved);
        }
        private readonly TelemetryPersistence _persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
    }
}
