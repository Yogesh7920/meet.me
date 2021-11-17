using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.Summary;

namespace Dashboard
{
    /// <summary>
    /// Class for sending data to the client side
    /// from the server side
    /// </summary>
    public class ServerToClientData
    {
        /// <summary>
        /// Parametric constructor to initialize the fields
        /// </summary>
        /// <param name="eventName"> The name of the event </param>
        /// <param name="objectToSend"> The object that is to be sent on the client side </param>
        public ServerToClientData(string eventName, SessionData sessionDataToSend, SummaryData summaryDataToSend, UserData user)
        {
            eventType = eventName;
            _user = user;
            sessionData = sessionDataToSend;
            summaryData = summaryDataToSend;
        }

       /// <summary>
       /// Default constructor for serialization
       /// </summary>
        public ServerToClientData()
        {
        }

        /// <summary>
        /// Method to access the UserData object 
        /// </summary>
        /// <returns> A UserData object containing the details of a user. </returns>
        public UserData GetUser()
        {
            return _user;
        }

        public string eventType;
        public SummaryData summaryData;
        public SessionData sessionData;
        public UserData _user;
    }
}
