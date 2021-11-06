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

        /// <params> 
        ///     takes the session data which contains the users list 
        ///     and whenever the session data changes, I get notified, 
        ///     based on it I can store the timestamp
        /// </params>

        /// <returns>
        ///     A dictionary of DateTime as key and userCount as value
        /// </returns>
        Dictionary< DateTime, int > UserCountVsTimeStamp(SessionData newSession){
            throw new NotImplementedException();
        }


        /// <summary>
        ///     returns a dictionary of UserData as key and chatCount as value
        ///     indicating chat count of each user.
        /// </summary>
        /// <params> Takes array of ChatContext object which contains information about Threads </params>
        /// <returns> 
        ///     Dictionary of userData as key and int as value
        /// </returns>
        Dictionary<UserData, int> UserVsChatCount(ChatContext[] AllMess){
            throw new NotImplementedException();
        }

        /// <summary> 
        ///     returns the list of UserData which indicates insincere members
        ///     in the session who were present in the session but for less 
        ///     than a certain minimum time
        /// </summary>
        /// <params> Takes the session data which contains the list of users </params>
        /// <returns> A list of type UserDat  </returns>
        List<UserData> irrelevantMembers(SessionData newSession){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     returns a dictionary of SessionData as key and Score(int) as value
        ///     which indicates score for each session
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        Dictionary<SessionData, int> sessionVsScore(List<ServerDataToSave> AllSessionData ){
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Gives the session wise analysis of the server. Show plots for users and session
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        Dictionary<SessionData, int > sessionVsUserCount(List<ServerDataToSave> AllSessionData){
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Gives the session wise analysis of the server. Show plots of chats vs session.
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        Dictionary<SessionData, int > sessionVsChatCount(List<ServerDataToSave> AllSessionData){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the SessionData when it changes
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        void OnAnalyticsChanged(SessionData newSession){
            throw new NotImplementedException();
        }

        public void SaveAllAnalytics(ChatContext[] AllMessages){
            throw new NotImplementedException();
        }

        public TelemetryAnalyticsModel getTelemetryAnalytics(ChatContext[] AllMessages){
            throw new NotImplementedException();
        }
    }
}