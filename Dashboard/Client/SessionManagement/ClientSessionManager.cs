/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the implementation of Client session manager.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Content;
using Dashboard.Server.Telemetry;
using Networking;
using ScreenSharing;
using Whiteboard;

namespace Dashboard.Client.SessionManagement
{
    public delegate void NotifyEndMeet();

    public delegate void NotifyAnalyticsCreated(SessionAnalytics analytics);

    public delegate void NotifySummaryCreated(string summary);

    /// <summary>
    ///     ClientSessionManager class is used to maintain the client side
    ///     session data and requests from the user. It communicates to the server session manager
    ///     to update the current session or to fetch summary and analytics.
    /// </summary>
    public class ClientSessionManager : IUXClientSessionManager, INotificationHandler
    {
        private readonly List<IClientSessionNotifications> _clients;
        private readonly ICommunicator _communicator;
        private readonly IContentClient _contentClient;
        private readonly ISerializer _serializer;
        private readonly IClientBoardStateManager clientBoardStateManager;
        private readonly string moduleIdentifier;

        private string _chatSummary;
        private SessionData _clientSessionData;

        private readonly ScreenShareClient _screenShareClient;

        private SessionAnalytics _sessionAnalytics;

        private UserData _user;

        /// <summary>
        ///     Default constructor that will initialize communicator, contentclient,
        ///     clientBoardStateManager and user side client data.
        /// </summary>
        public ClientSessionManager()
        {
            _ = new TraceManager();
            moduleIdentifier = "Dashboard";
            _serializer = new Serializer();
            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe(moduleIdentifier, this);
            _contentClient = ContentClientFactory.GetInstance();
            clientBoardStateManager = ClientBoardStateManager.Instance;
            clientBoardStateManager.Start();

            if (_clients == null) _clients = new List<IClientSessionNotifications>();
            _clientSessionData = null;
            _user = null;
            _chatSummary = null;

            _screenShareClient = ScreenShareFactory.GetScreenShareClient();
            if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E") return;
        }

        /// <summary>
        ///     Added for testing the Module. The communicator can be set manually.
        /// </summary>
        /// <param name="communicator">
        ///     Test communicator to test the functionality
        /// </param>
        public ClientSessionManager(ICommunicator communicator, IClientBoardStateManager whiteboardInstance = null)
        {
            _ = new TraceManager();
            moduleIdentifier = "Dashboard";
            _serializer = new Serializer();
            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
            _screenShareClient = ScreenShareFactory.GetScreenShareClient();

            if (whiteboardInstance != null)
                clientBoardStateManager = whiteboardInstance;
            else
                clientBoardStateManager = ClientBoardStateManager.Instance;
            clientBoardStateManager.Start();


            if (_clients == null) _clients = new List<IClientSessionNotifications>();
            _clientSessionData = new SessionData();
            _chatSummary = null;

            _screenShareClient = ScreenShareFactory.GetScreenShareClient();
        }

        /// <summary>
        ///     This function will handle the serialized data received from the networking module.
        ///     It will first deserialize and then handle the appropriate cases.
        /// </summary>
        /// <param name="serializedData"> The serialized string sent by the networking module </param>
        public void OnDataReceived(string serializedData)
        {
            Trace.WriteLine("[Client Dashboard] Received data from the server.");

            if (serializedData == null)
            {
                Trace.WriteLine("[Client Dashboard] Null string received from the server.");
                return;
            }

            // Deserialize the data when it arrives
            var deserializedObject = _serializer.Deserialize<ServerToClientData>(serializedData);

            // check the event type and get the object sent from the server side
            var eventType = deserializedObject.eventType;

            // based on the type of event, calling the appropriate functions 
            switch (eventType)
            {
                case "addClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "getSummary":
                    UpdateSummary(deserializedObject);
                    return;

                case "getAnalytics":
                    UpdateAnalytics(deserializedObject);
                    return;

                case "removeClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "endMeet":
                    _communicator.Stop();
                    _screenShareClient.Dispose();
                    MeetingEnded?.Invoke();
                    return;

                default:
                    Trace.WriteLine("Received Invalid event type from the server");
                    return;
            }
        }

        /// <summary>
        ///     Adds a user to the meeting.
        /// </summary>
        /// <param name="ipAddress"> IP Address of the meeting. </param>
        /// <param name="ports"> port number. </param>
        /// <param name="username"> Name of the user. </param>
        /// <returns> Boolean denoting the success or failure whether the user was added. </returns>
        public bool AddClient(string ipAddress, int port, string username)
        {
            // Null or whitespace named users are not allowed
            if (string.IsNullOrWhiteSpace(username))
            {
                Trace.WriteLine("[Client Dashboard] Null user name or whitespace given.");
                return false;
            }

            ipAddress = ipAddress.Trim();

            lock (this)
            {
                // trying to connect
                var connectionStatus = _communicator.Start(ipAddress, port.ToString());

                // if the IP address and/or the port number are incorrect
                if (connectionStatus == "0")
                {
                    Trace.WriteLine(
                        "[Client Dashboard] Incorrect credentials given. Client cannot connect to the server.");
                    return false;
                }
            }

            // upon successfull connection, the request to add the client is sent to the server side.
            SendDataToServer("addClient", username);
            Trace.WriteLine("[Client Dashboard] Adding Client to the session");
            return true;
        }

        /// <summary>
        ///     End the meeting for all, creating and storing the summary and analytics.
        /// </summary>
        public void EndMeet()
        {
            Trace.WriteLine("[Client Dashboard] Asking the server to end the meeting.");
            SendDataToServer("endMeet", _user.username, _user.userID);
        }

        /// <summary>
        ///     Gather analytics of the users and messages.
        /// </summary>
        public void GetAnalytics()
        {
            Trace.WriteLine("[Client Dashboard] Getting the analytics from the server.");

            SendDataToServer("getAnalytics", _user.username, _user.userID);
        }


        /// <summary>
        ///     Get the summary of the chats that were sent from the start of the
        ///     meet till the function was called.
        /// </summary>
        /// <returns> Summary of the chats as a string. </returns>
        public void GetSummary()
        {
            Trace.WriteLine("[Client Dashboard] Getting the summary from the server.");

            SendDataToServer("getSummary", _user.username, _user.userID);
        }

        /// <summary>
        ///     Fetches the Userdata object of the client.
        /// </summary>
        /// <returns>Returns the userData object of the client.</returns>
        public UserData GetUser()
        {
            return _user;
        }

        /// <summary>
        ///     Removes the user from the meeting by deleting their
        ///     data from the session.
        /// </summary>
        public void RemoveClient()
        {
            Trace.WriteLine("[Client Dashboard] Asking the server to remove client from the server side.");

            SendDataToServer("removeClient", _user.username, _user.userID);
            Trace.WriteLine("[Client Dashboard] Stopping the network communicator.");

            _communicator.Stop();
            Trace.WriteLine("[Client Dashboard] Disposing the Screen Share Client.");
            _screenShareClient.Dispose();

            Trace.WriteLine("[Client Dashboard] Removed the client from the client side.");
            
            
        }

        /// <summary>
        ///     Used to subcribe for any changes in the
        ///     Session object.
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The identifier of the subscriber. </param>
        public void SubscribeSession(IClientSessionNotifications listener)
        {
            lock (this)
            {
                Trace.WriteLine("[Client Dashboard] A subscription was made for client side session data");
                _clients.Add(listener);
            }
        }

        public event NotifyEndMeet MeetingEnded;
        public event NotifySummaryCreated SummaryCreated;
        public event NotifyAnalyticsCreated AnalyticsCreated;

        /// <summary>
        ///     Used to fetch the sessionData for the client. Helpful for testing and debugging.
        /// </summary>
        /// <returns>A SessionData object containing the list of users in the session.</returns>
        public SessionData GetSessionData()
        {
            return _clientSessionData;
        }

        public SessionAnalytics GetStoredAnalytics()
        {
            return _sessionAnalytics;
        }

        /// <summary>
        ///     Used to fetch the stored summary for the client. Helpful for testing and debugging.
        /// </summary>
        /// <returns>The summary of the session as a string.</returns>
        public string GetStoredSummary()
        {
            return _chatSummary;
        }

        /// <summary>
        ///     Will Notifiy UX about the changes in the Session
        /// </summary>
        public void NotifyUXSession()
        {
            for (var i = 0; i < _clients.Count; ++i)
                lock (this)
                {
                    Trace.WriteLine("[Client Dashboard] Notifying UX about the session change.");

                    _clients[i].OnClientSessionChanged(_clientSessionData);
                }
        }

        /// <summary>
        ///     A helper function that sends data from Client to the server side. The data consists
        ///     of the event type and the user who requested for that event.
        /// </summary>
        /// <param name="eventName"> The type of the event. </param>
        /// <param name="username"> The username of the user who requested</param>
        /// <param name="userID"> The user ID of the user, if the user has not yet beend created then the default value is -1. </param>
        private void SendDataToServer(string eventName, string username, int userID = -1)
        {
            ClientToServerData clientToServerData;
            lock (this)
            {
                clientToServerData = new ClientToServerData(eventName, username, userID);
                var serializedData = _serializer.Serialize(clientToServerData);
                _communicator.Send(serializedData, moduleIdentifier);
            }
        }

        /// <summary>
        ///     Used to set/change the Client side UserData Object for testing and deubgging purposes.
        /// </summary>
        /// <param name="userName"> The username of the user.</param>
        /// <param name="userID"> The user ID of the user, the default value is -1. </param>
        public void SetUser(string userName, int userID = 1)
        {
            _user = new UserData(userName, userID);
        }

        /// <summary>
        ///     Used to set/change the users list for testing and deubgging purposes.
        /// </summary>
        /// <param name="users">The list of UserData object that the sessionData will be asssignet to.</param>
        public void SetSessionUsers(List<UserData> users)
        {
            _clientSessionData.users = users;
        }

        private void UpdateAnalytics(ServerToClientData receivedData)
        {
            _sessionAnalytics = receivedData.sessionAnalytics;
            var receiveduser = receivedData.GetUser();

            Trace.WriteLine("[Client Dashboard] Notifying UX about the Analytics.");

            AnalyticsCreated?.Invoke(_sessionAnalytics);
        }

        /// <summary>
        ///     Updates the locally stored summary at the client side to the summary received from the
        ///     server side. The summary will only be updated fro the user who requsted it.
        /// </summary>
        /// <param name="receivedData">
        ///     A ServerToClientData object that contains the summary
        ///     created at the server side of the session manager.
        /// </param>
        private void UpdateSummary(ServerToClientData receivedData)
        {
            // Extract the summary string and the user.
            var receivedSummary = receivedData.summaryData;
            var receivedUser = receivedData.GetUser();

            if (receivedSummary == null) Trace.WriteLine("[Client Dashboard] Null summary received.");

            // check if the current user is the one who requested to get the summary
            if (receivedUser.userID == _user.userID)
                lock (this)
                {
                    _chatSummary = receivedSummary.summary;
                    Trace.WriteLine("[Client Dashboard] Notifying UX about the summary.");
                    SummaryCreated?.Invoke(_chatSummary);
                }
        }

        /// <summary>
        ///     Compares the server side session data to the client side and update the
        ///     client side data if they are different.
        /// </summary>
        /// <param name="recievedSessionData"> The sessionData received from the server side. </param>
        private void UpdateClientSessionData(ServerToClientData receivedData)
        {
            // fetching the session data and user received from the server side
            var receivedSessionData = receivedData.sessionData;
            var user = receivedData.GetUser();

            //Debug.Assert(receivedSessionData.users != null);

            // if there was no change in the data then nothing needs to be done
            if (receivedSessionData != null && _clientSessionData != null &&
                receivedSessionData.users.Equals(_clientSessionData.users))
                return;

            // a null _user denotes that the user is new and has not be set because all 
            // the old user (already present in the meeting) have their _user set.
            if (_user == null)
            {
                _user = user;

                Trace.WriteLine("[Client Dashboard] Client added to the client session.");

                clientBoardStateManager.SetUser(user.userID.ToString());
                Trace.WriteLine("[Client Dashboard] Whiteboard's user ID set.");

                if (Environment.GetEnvironmentVariable("TEST_MODE") != "E2E")
                    _screenShareClient.SetUser(user.userID.ToString(), user.username);

                Trace.WriteLine("[Client Dashboard] ScreenShare's user ID and username set.");

                ContentClientFactory.SetUser(user.userID);
                Trace.WriteLine("[Client Dashboard] Content's user ID set.");
            }

            // The user received from the server side is equal to _user only in the case of 
            // client departure. So, the _user and received session data are set to null to indicate this departure
            else if (_user.Equals(user))
            {
                _user = null;
                Trace.WriteLine("[Client Dashboard] Client removed from the client session data.");
                receivedSessionData = null;
            }

            // update the sesseon data on the client side and notify the UX about it.
            lock (this)
            {
                _clientSessionData = receivedSessionData;
            }

            NotifyUXSession();
        }
    }
}