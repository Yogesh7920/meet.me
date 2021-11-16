using System;
using System.Collections.Generic;
using Dashboard.Server.SessionManagement;
using Content;

namespace Dashboard.Server.Telemetry{
    
    public class Telemetry: ITelemetry
    {
        /// <summary>
        ///     returns a dictionary with DateTime as key and int as value
        ///     which indicates UserCount at corresponding DateTime 
        /// </summary>
        /// <params name= "new_session"> 
        ///     takes the session data which contains the users list 
        ///     and whenever the session data changes, Telemetry get notified, 
        ///     based on it timestamp can be stored.
        /// </params>
        /// <returns>
        ///     A dictionary of DateTime as key and userCount as value
        /// </returns>
        public Dictionary< DateTime, int > GetUserCountVsTimeStamp(SessionData new_session){
            
            throw new NotImplementedException();

        }

        /// <summary>
        ///     returns a dictionary of UserID as key and chatCount as value
        ///     indicating chat count of each user.
        /// </summary>
        /// <params name="all_messages"> Takes array of ChatContext object which contains information about Threads </params>
        /// <returns> 
        ///     Dictionary of userID as key and int as value
        /// </returns>
        public Dictionary<int, int> GetUserVsChatCount(ChatContext[] all_messages){
            Dictionary<int, int> UserChatCountDic= new Dictionary<int, int>();
            foreach(ChatContext CurrThread in all_messages){
                foreach(ReceiveMessageData CurrMessage in CurrThread.MsgList){
                    UserChatCountDic[CurrMessage.SenderId]++;
                }
            }
            return UserChatCountDic;
        }

        /// <summary> 
        ///     returns the list of UserID which indicates insincere members
        ///     in the session, i.e. those who were present in the session but for less 
        ///     than a certain minimum time
        /// </summary>
        /// <params name="new_session"> Takes the session data which contains the list of users </params>
        /// <returns> A list of type int(userID)  </returns>
        public List<int> GetInsincereMembers(SessionData new_session){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     appends the current session data in the previous ServerDataToSave object
        /// </summary>
        /// <params name="previous_sessions_data"> Takes ServerDataToSave, which contains information about previous sessions </params>
        /// <returns>
        ///     returns the updated ServerDataToSave object
        /// </returns>
        public ServerDataToSave UpdateServerData(ServerDataToSave previous_sessions_data ){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     To get any change in the SessionData
        /// </summary>
        /// <params name="new_session"> Received new SessionData </params>
        void OnAnalyticsChanged(SessionData newSession){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     simplifies the ChatContext and saved by persistance module.
        /// </summary>
        /// <params name="all_messages"> Array of ChatContext objects which contains information about messages of each thread </params>    
        public void SaveAnalytics(ChatContext[] all_messages){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Used by SM to get the SessionAnalytics to transfer
        ///     back to UX module to display the analytics
        /// </summary>
        /// <params> Array of ChatContext objects which contains information about messages of each thread </params>
        /// <returns>
        ///     Returns SessionAnalytics object which contains analytics of session
        /// </returns>
        public SessionAnalytics getTelemetryAnalytics(ChatContext[] all_messages){
            throw new NotImplementedException();
        }
    }
}