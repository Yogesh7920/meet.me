using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Client.SessionManagement
{
    using Dashboard.Server.Telemetry;
    public interface IUXClientSessionManager
    {

        /// <summary>
        /// Adds a user to the meeting.
        /// </summary>
        /// <param name="ipAddress"> IP Address of the meeting. </param>
        /// <param name="ports"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure whether the user was added. </returns>
        bool AddClient(string ipAddress,
                       int ports,
                       string username);

        /// <summary>
        /// Removes the user from the meeting by deleting their 
        /// data from the session.
        /// </summary>
        void RemoveClient();

        /// <summary>
        /// End the meeting for all, creating and storing the summary and analytics.
        /// </summary>
        void EndMeet();

        /// <summary>
        /// Get the summary of the chats that were sent from the start of the
        /// meet till the function was called.
        /// </summary>
        /// <returns> Summary of the chats as a string. </returns>
        string GetSummary();

        /// <summary>
        /// Used to subcribe for any changes in the 
        /// Session object.
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The identifier of the subscriber. </param>
        void SubscribeSession(ISessionNotifications listener, string identifier);

        /// <summary>
        /// Gather analytics of the users and messages.
        /// </summary>
        ITelemetryAnalysisModel GetAnalytics(); 
    }
}

