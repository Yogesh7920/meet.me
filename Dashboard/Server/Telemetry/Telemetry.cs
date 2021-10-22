using System;
using System.Collections.Generic;
using Dashboard.Server.SessionManagement;
using Content;

namespace Dashboard.Server.Telemetry{
    
    public class Telemetry:ITelemetryNotifications
    {
        /// <summary>
        ///     plots the histogram of number of users vs time 
        /// </summary>
        /// <params> takes the session data which contains the users list and whenever the session data changes, I get notified, based on it I can store the timestamp
        void user_time(SessionData newSession){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     plots a pie chart for number of messages sent by each user
        /// </summary>
        /// <params> Takes the list of Threads </params>
        void messages_user(List<Content.models.Threads> AllMess){
            throw new NotImplementedException();
        }

        /// <summary> 
        ///     returns the list of insincere members in the session who were present in the session but for less than a certain minimum time
        /// </summary>
        /// <params> Takes the session data </params>
        /// <returns> UserData list </returns>
        List<UserData> irrelevantMembers(SessionData newSession){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gives the session wise analysis of the server. Show visuals for score of each session
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        void sessionVsScore(List<ServerData> AllSessionData ){
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Gives the session wise analysis of the server. Show plots for users and session
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        void sesssionVsUsers(List<ServerData> AllSessionData){
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Gives the session wise analysis of the server. Show plots of chats vs session.
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        void sessionVsChats(List<ServerData> AllSessionData){
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the SessionData when it changes
        /// </summary>
        /// <params> Takes the list of ServerData </params>
        void OnAnalyticsChanged(SessionData newSession){
            throw new NotImplementedException();
        }
    }
}