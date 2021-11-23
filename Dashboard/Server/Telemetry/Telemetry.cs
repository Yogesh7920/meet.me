using System;
using System.Collections.Generic;
using Dashboard.Server.SessionManagement;
using Content;
using Dashboard.Server.Persistence;

namespace Dashboard.Server.Telemetry{
    ///<summary>
    /// All analytics are done in this class
    ///</summary>
    public class Telemetry: ITelemetry
    {
        // public void Telemetry
        // {
        //     _sessionmanager.subscribe();
        // }
        /// <summary>
        ///     constructs a dictionary with DateTime as key and int as value
        ///     which indicates UserCount at corresponding DateTime 
        /// </summary>
        /// <params name= "newSession"> 
        ///     takes the session data which contains the users list 
        ///     and whenever the session data changes, Telemetry get notified, 
        ///     based on it timestamp can be stored.
        /// </params>
        public void GetUserCountVsTimeStamp(SessionData newSession, DateTime currTime)
        {
            userCountAtEachTimeStamp[currTime] = newSession.users.Count;
        }

        /// <summary>
        ///     constructs the dictionary of UserID as key and chatCount as value
        ///     indicating chat count of each user.
        /// </summary>
        /// <params name="allMessages"> Takes array of ChatContext object which contains information about Threads </params>
        public void GetUserVsChatCount(ChatContext[] allMessages)
        {
            foreach(ChatContext currThread in allMessages)
            {
                foreach(ReceiveMessageData currMessage in currThread.MsgList)
                {
                    if(userIdChatCountDic.ContainsKey(currMessage.SenderId)) userIdChatCountDic[currMessage.SenderId]++;
                    else
                    {
                        userIdChatCountDic.Add(currMessage.SenderId, 1);
                    }
                }
            }
        }

        /// <summary> 
        ///     Calculates the enter and exit time for each user. Whenever SessionData
        ///     changes, that means any user has either entered or exited.
        /// </summary>
        /// <params name="newSession"> Takes the session data which contains the list of users </params>
        public void CalculateEnterExitTimes(SessionData newSession, DateTime currTime)
        {
            foreach(UserData user_i in newSession.users )
            {
                if(userEnterTime.ContainsKey(user_i)==false)
                {
                    userEnterTime[user_i]= currTime;
                }
            }
            // if user is in userEnterTime but not in users list, that means he left the meeting.
            foreach(KeyValuePair<UserData,DateTime> user_i in userEnterTime){
                if(newSession.users.Contains(user_i.Key)==false && userExitTime.ContainsKey(user_i.Key)==false ){
                    userExitTime[user_i.Key]=currTime;
                }
            }

        }

        /// <summary>
        ///     Constructs the insincereMembers list from userEnterTime and useExitTime dictionary
        /// </summary>
        public void GetInsincereMembers()
        {
            foreach(KeyValuePair<UserData,DateTime> user_i in userEnterTime)
            {
                UserData  currUser = user_i.Key;
                // if difference of exit and enter time is less than 30 min.
                if(userExitTime.ContainsKey(currUser) && userExitTime[currUser].Subtract(user_i.Value).TotalMinutes < thresholdTime)
                {
                    int id = currUser.userID;
                    insincereMembers.Add(id);
                }
            }
        }

        /// <summary>
        ///     appends the current session data in the previous ServerDataToSave object
        /// </summary>
        /// <params name="totalUsers"> Total number of users in the current session </params>
        /// <params name="totalChats"> Total chats in the current session </params>
        public void UpdateServerData(int totalUsers, int totalChats ){
            // retrieve the previous server data till previous session
            ServerDataToSave serverData = _persistence.RetrieveAllSeverData(); 
            serverData.sessionCount++;
            // current session data
            SessionSummary currSessionSummary = new SessionSummary();
            currSessionSummary.userCount = totalUsers;
            currSessionSummary.chatCount = totalChats;
            currSessionSummary.score = totalChats * totalUsers;
            serverData.allSessionsSummary.Add(currSessionSummary);
            _persistence.SaveServerData(serverData);
        }

        /// <summary>
        ///     To get any change in the SessionData
        /// </summary>
        /// <params name="newSession"> Received new SessionData </params>
        public void OnAnalyticsChanged(SessionData newSession, DateTime time)
        {
            GetUserCountVsTimeStamp(newSession, time);
            CalculateEnterExitTimes(newSession, time);
        }

        /// <summary>
        ///     Used to simplify the ChatContext and saved all analytics when the session is over.
        /// </summary>
        /// <params name="allMessages"> Array of ChatContext objects which contains information about messages of each thread </params>    
        public void SaveAnalytics(ChatContext[] allMessages)
        {
            // save the session data
            GetUserVsChatCount(allMessages);
            GetInsincereMembers();
            SessionAnalytics sessionAnalyticsToSave = new SessionAnalytics();
            sessionAnalyticsToSave.chatCountForEachUser=userIdChatCountDic;
            sessionAnalyticsToSave.userCountAtAnyTime= userCountAtEachTimeStamp;
            sessionAnalyticsToSave.insincereMembers=insincereMembers;
            _persistence.Save(sessionAnalyticsToSave);
            // saving server data
            int totalChats=0;
            int totalUsers=0;
            foreach(KeyValuePair<int,int> user_i in userIdChatCountDic){
                totalChats+=user_i.Value;
                totalUsers+=1;
            }
            UpdateServerData(totalUsers, totalChats);
        }

        /// <summary>
        ///     get the SessionAnalytics to transfer
        ///     back to UX module to display the analytics
        /// </summary>
        /// <params> Array of ChatContext objects which contains information about messages of each thread </params>
        /// <returns>
        ///     Returns SessionAnalytics object which contains analytics of session
        /// </returns>
        public SessionAnalytics GetTelemetryAnalytics(ChatContext[] allMessages)
        {
            GetUserVsChatCount(allMessages);
            GetInsincereMembers();
            SessionAnalytics sessionAnalyticsToSend = new SessionAnalytics();
            sessionAnalyticsToSend.chatCountForEachUser=userIdChatCountDic;
            sessionAnalyticsToSend.userCountAtAnyTime= userCountAtEachTimeStamp;
            sessionAnalyticsToSend.insincereMembers=insincereMembers;
            return sessionAnalyticsToSend;
        }

        public Dictionary<DateTime, int> userCountAtEachTimeStamp = new Dictionary<DateTime, int>();
        public Dictionary<UserData,DateTime> userEnterTime=new Dictionary<UserData, DateTime>();
        public Dictionary<UserData,DateTime> userExitTime=new Dictionary<UserData, DateTime>();
        public Dictionary<int, int> userIdChatCountDic= new Dictionary<int, int>();
        public List<int> insincereMembers= new List<int>();
        private readonly ITelemetryPersistence _persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
        private int thresholdTime = 30;
        // private ITelemetrySessionManager _sm = new ITelemetrySessionManager();
    }
}