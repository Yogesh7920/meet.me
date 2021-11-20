using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Telemetry;
using Dashboard;

namespace Testing.Dashboard
{
    [TestClass]
    public class Telemetry
    {
        [TestMethod]
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
            OnAnalyticsChanged(session1);
            int userCount1 = UserCountAtEachTimeStamp[DateTime.Now];
            bool check1;
            // total users in session1 is 1
            if(userCount1==1) check1=true;
            OnAnlyticsChanged(session2);
            int userCount2= UserCountAtEachTimeStamp[DateTime.Now];
            bool check2;
            // total users in session2 is 2.
            if(userCount2==2) check2=true;
            //Assert
            Assert.isTrue(check1 && check2);
            
        }

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
            OnAnalyticsChanged(session1);
            OnAnalyticsChanged(session2);
            int elementAtZero = insincereMembers.ElementAt(0);
            //Assert
            Assert.AreEqual(elementAtZero,1);
        }

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
            SaveAnalytics(allMessages);
            int chatCountUser1= userIdChatCountDic[1];
            bool check1=false;
            if(chatCountUser1==2)
            {
                check1=true;
            }
            int chatCountUser2= userIdChatCountDic[2];
            bool check2=false;
            if(chatCountUser2==1)
            {
                check2=true;
            }
            
            //Assert
            Assert.isTrue(check1 && check2);

        }
        
    }
}
