using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Dashboard.Client.SessionManagement 
{
    using Dashboard.Server.Telemetry;
    public class ClientSessionManager : IUXClientSessionManager
    {

        public ClientSessionManager()
        {
            Session session = new Session();
            session.TraceListener();
        }
        /// <summary>
        /// Adds a user to the meeting.
        /// </summary>
        /// <param name="ipAddress"> IP Address of the meeting. </param>
        /// <param name="ports"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure whether the user was added. </returns>
        public bool AddClient(string ipAddress, int ports, string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the user from the meeting by deleting their 
        /// data from the session.
        /// </summary>
        public void RemoveClient()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// End the meeting for all, creating and storing the summary and analytics.
        /// </summary>
        public void EndMeet()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the summary of the chats that were sent from the start of the
        /// meet till the function was called.
        /// </summary>
        /// <returns> Summary of the chats as a string. </returns>
        public string GetSummary()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to subcribe for any changes in the 
        /// Session object.
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The identifier of the subscriber. </param>
        public void SubscribeSession(ISessionNotifications listener, string identifier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gather analytics of the users and messages.
        /// </summary>
        public ITelemetryAnalysisModel GetAnalytics()
        {
            // the return type will be an analytics object yet to be decided.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Will Notifiy UX about the changes in the Session
        /// </summary>
        public void NotifyUXSession()
        {

        }

        public SessionData _sessionObject;
    }
}
