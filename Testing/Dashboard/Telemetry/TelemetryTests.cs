using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Telemetry;
using Dashboard.Server.Persistence;
using Content;
using Dashboard;

namespace Testing.Dashboard
{
    public class Telemetry
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
            _telemetry.OnAnalyticsChanged(session1);
            int userCount1 = _telemetry.UserCountAtEachTimeStamp[DateTime.Now];
            bool check1;
            // total users in session1 is 1
            if(userCount1==1) check1=true;
            _telemetry.OnAnlyticsChanged(session2);
            int userCount2= _telemetry.UserCountAtEachTimeStamp[DateTime.Now];
            bool check2;
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
            _telemetry.OnAnalyticsChanged(session1);
            _telemetry.OnAnalyticsChanged(session2);
            int elementAtZero = _telemetry.insincereMembers.ElementAt(0);
            //Assert
            Assert.AreEqual(elementAtZero,1);
        }

        [Test]
        public void SaveAnalytics_GetUserVsChatCount_CorrectChatCountForAnyUser()
        {
            // Arrange
            ReceiveMessageData  message1= new ReceiveMessageData();
            message1.Message= "Hello from user 1";
            messsage1.SenderId=1;
            ReceiveMessageData  message2= new ReceiveMessageData();
            message1.Message= "Another Hello from user 1";
            messsage1.SenderId=1;
            ReceiveMessageData  message3= new ReceiveMessageData();
            message1.Message= "Hello from user 2";
            messsage1.SenderId=2;

            List<ReceivedMessageData> msgList1 = new List<ReceivedMessageData>();
            msgList1.Add(message1);
            msgList1.Add(message2);
            List<ReceivedMessageData> msgList2 = new List<ReceivedMessageData>();
            msgList2.Add(message3);

            ChatContext[] allMessages = new ChatContext[2];
            ChatConext chat1 = new ChatConext();
            chat1.CreationTime = DateTime.Now;
            chat1.MsgList=msgList1;
            ChatConext chat2 = new ChatConext();
            chat2.CreationTime = DateTime.Now;
            chat2.MsgList=msgList2;
            allMessages[0]=chat1;
            allMessages[1]=chat2;

            //Act
            _telemetry.SaveAnalytics(allMessages);
            int chatCountUser1= _telemetry.userIdChatCountDic[1];
            bool check1=false;
            if(chatCountUser1==2)
            {
                check1=true;
            }
            int chatCountUser2= _telemetry.userIdChatCountDic[2];
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
            ServerDataToSave serverData= RetrieveAllSeverData();
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
            ResponseEntity response = SaveServerData(serverData);
            Assert.IsTrue(response.IsSaved);
        }
        private Telemetry _telemetry = new Telemetry();
    }
}
