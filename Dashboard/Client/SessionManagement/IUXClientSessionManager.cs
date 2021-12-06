

using System;
/// <author> Rajeev Goyal </author>
/// <created> 24/10/2021 </created>
/// <summary>
/// This file contains the interface for UX to access Client session manager's methods and fields.
/// </summary>
namespace Dashboard.Client.SessionManagement
{
    public interface IUXClientSessionManager
    {
        /// <summary>
        ///     Adds a user to the meeting.
        /// </summary>
        /// <param name="ipAddress"> IP Address of the meeting. </param>
        /// <param name="ports"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure whether the user was added. </returns>
        bool AddClient(string ipAddress,
            int ports,
            string username);

        /// <summary>
        ///     Removes the user from the meeting by deleting their
        ///     data from the session.
        /// </summary>
        void RemoveClient();

        /// <summary>
        ///     End the meeting for all, creating and storing the summary and analytics.
        /// </summary>
        void EndMeet();

        /// <summary>
        ///     Get the summary of the chats that were sent from the start of the
        ///     meet till the function was called.
        /// </summary>
        /// <returns> Summary of the chats as a string. </returns>
        void GetSummary();

        /// <summary>
        ///     Used to subcribe for any changes in the
        ///     Session object.
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        void SubscribeSession(IClientSessionNotifications listener);

        /// <summary>
        ///     Gather analytics of the users and messages.
        /// </summary>
        void GetAnalytics();

        /// <summary>
        ///     Fetches the user data object from the client session manager.
        /// </summary>
        /// <returns>A userData object for that paritcular client.</returns>
        UserData GetUser();

        /// <summary>
        ///     Fetches the session data object from the client session manager.
        /// </summary>
        /// <returns> A session data object for that paritcular client. </returns>
        //SessionData GetSessionData() 
        //{
        //    return null;
        //}

        // Event for notifying summary creation 
        public event NotifySummaryCreated SummaryCreated;

        // Event for notifying the end of the meeting on the client side
        public event NotifyEndMeet MeetingEnded;

        // Event for notifying the creation of anlalytics to the client UX.
        public event NotifyAnalyticsCreated AnalyticsCreated;
    }
}