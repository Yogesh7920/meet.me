using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;


namespace Dashboard.Client.SessionManagement 
{
    using Dashboard.Server.Telemetry;

    /// <summary>
    /// ClientSessionManager class is used to maintain the client side 
    /// session data and requests from the user. It communicates to the server session manager 
    /// to update the current session or to fetch summary and analytics.
    /// </summary>
    public class ClientSessionManager : IUXClientSessionManager, INotificationHandler
    {
        /// <summary>
        /// Default constructor that will create a new SessionData object,
        /// trace listener and the list for maintaining the subscribers for SessionData
        /// </summary>
        public ClientSessionManager()
        {
            _serializer = new Serializer();
            _communicator = CommunicationFactory.GetCommunicator();
            Session session = new();
            session.TraceListener();
           

            if(_clients == null)
            {
                _clients = new List<IClientSessionNotifications>();
            }
            _clientSessionData = new SessionData();
            moduleIdentifier = "clientSessionManager";
            chatSummary = null;
        }

        /// <summary>
        /// Added for testing the Module
        /// </summary>
        /// <param name="communicator">
        /// Test communicator to test the functionality
        /// </param>
        public ClientSessionManager(ICommunicator communicator)
        {
            _serializer = new Serializer();
            _communicator = communicator;
            Session session = new();
            session.TraceListener();


            if (_clients == null)
            {
                _clients = new List<IClientSessionNotifications>();
            }
            _clientSessionData = new SessionData();
            moduleIdentifier = "clientSessionManager";
            chatSummary = null;
        }

        /// <summary>
        /// Adds a user to the meeting.
        /// </summary>
        /// <param name="ipAddress"> IP Address of the meeting. </param>
        /// <param name="ports"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure whether the user was added. </returns>
        public bool AddClient(string ipAddress, int port, string username)
        {

            string serializedClientName;
            
            lock (this)
            {
                string connectionStatus = _communicator.Start(ipAddress, port.ToString());

                // if the IP address and/or the port number are incorrect
                if (connectionStatus == "0")
                {
                    return false;
                }

                ClientToServerData clientName = new("addClient", username);
                serializedClientName = _serializer.Serialize<ClientToServerData>(clientName);
            }
            
            _communicator.Send(serializedClientName,moduleIdentifier);
            return true;
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
            string summary = "";
            ClientToServerData clientToServerData = new("getSummary", _user.username, _user.userID);
            string serializedData = _serializer.Serialize<ClientToServerData>(clientToServerData);
            _communicator.Send(serializedData, moduleIdentifier);
            
            // This loop will run till the summary is received from the server side.
            while(chatSummary == null)
            {

            }

            lock(this)
            {
                summary = chatSummary;
                chatSummary = null;
            }
            return summary;
        }

        /// <summary>
        /// Used to subcribe for any changes in the 
        /// Session object.
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The identifier of the subscriber. </param>
        public void SubscribeSession(IClientSessionNotifications listener)
        {
            lock(this)
            {
                _clients.Add(listener);
            }
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
            for(int i=0;i<_clients.Count;++i)
            {
                lock(this)
                {
                    _clients[i].OnClientSessionChanged(_clientSessionData);
                }
            }
        }

        /// <summary>
        /// This function will handle the serialized data received from the networking module.
        /// It will first deserialize and then handle the appropriate cases.
        /// </summary>
        /// <param name="serializedData"> The serialized string sent by the networking module </param>
        public void OnDataReceived(string serializedData)
        {
            // Deserialize the data when it arrives
            ServerToClientData deserializedObject = _serializer.Deserialize<ServerToClientData>(serializedData);

            // check the event type and get the object sent from the server side
            string eventType = deserializedObject.eventType;

            // based on the type of event, calling the appropriate functions 
            switch(eventType)
            {
                case "addClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "getSummary":
                    UpdateSummary(deserializedObject);
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates the locally stored summary at the client side to the summary received from the 
        /// server side. The summary will only be updated fro the user who requsted it.
        /// </summary>
        /// <param name="receivedData"> A ServerToClientData object that contains the summary 
        /// created at the server side of the session manager.</param>
        private void UpdateSummary(ServerToClientData receivedData)
        {
            // Extract the summary string and the user.
            SummaryData receivedSummary = (SummaryData) receivedData.GetObject();
            UserData receivedUser = receivedData.GetUser();

            // check if the current user is the one who requested to get the 
            // summary
            if(receivedUser.userID == _user.userID)
            {
                lock(this)
                {
                    chatSummary = receivedSummary.summary;
                }
            }
        }

        /// <summary>
        /// Compares the server side session data to the client side and update the 
        /// client side data if they are different.
        /// </summary>
        /// <param name="recievedSessionData"> The sessionData received from the server side. </param>
        private void UpdateClientSessionData(ServerToClientData receivedData)
        {
            SessionData recievedSessionData = (SessionData)receivedData.GetObject();
            UserData user = receivedData.GetUser();

            if (recievedSessionData == _clientSessionData)
                return;

            if(_clientSessionData == null)
            {
                _user = user;
            }
            lock(this)
            {
                _clientSessionData = (SessionData)recievedSessionData;
            }
            NotifyUXSession();
        }

        public SessionData _clientSessionData;
        private readonly ICommunicator _communicator;
        private readonly ISerializer _serializer;
        private readonly string moduleIdentifier;
        private readonly List<IClientSessionNotifications> _clients;
        private string chatSummary;
        private UserData _user;
    }
}
