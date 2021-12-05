/// <author> Rajeev Goyal </author>
/// <created> 24/10/2021 </created>
/// <summary>
/// This file contains the ServerToClientData class whose isntance are used to send data from server 
/// to client side.
/// </summary>

using Dashboard.Server.Telemetry;

namespace Dashboard
{
    /// <summary>
    ///     Class for sending data to the client side
    ///     from the server side
    /// </summary>
    public class ServerToClientData
    {
        public UserData _user;

        public string eventType;
        public SessionAnalytics sessionAnalytics;
        public SessionData sessionData;
        public SummaryData summaryData;

        /// <summary>
        ///     Parametric constructor to initialize the fields
        /// </summary>
        /// <param name="eventName"> The name of the event </param>
        /// <param name="objectToSend"> The object that is to be sent on the client side </param>
        public ServerToClientData(string eventName, SessionData sessionDataToSend, SummaryData summaryDataToSend,
            SessionAnalytics sessionAnalyticsToSend, UserData user)
        {
            // SessionAnalytics sessionAnalyticsToSend
            eventType = eventName;
            _user = user;
            sessionData = sessionDataToSend;
            summaryData = summaryDataToSend;
            sessionAnalytics = sessionAnalyticsToSend;
        }

        /// <summary>
        ///     Default constructor for serialization
        /// </summary>
        public ServerToClientData()
        {
        }

        /// <summary>
        ///     Method to access the UserData object
        /// </summary>
        /// <returns> A UserData object containing the details of a user. </returns>
        public UserData GetUser()
        {
            return _user;
        }
    }
}