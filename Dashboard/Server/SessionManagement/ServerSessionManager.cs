using Content;
using Dashboard.Server.Summary;
using Networking;
using ScreenSharing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Whiteboard;

namespace Dashboard.Server.SessionManagement
{
    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        /// <summary>
        /// Constructor for the ServerSessionManager, calls the 
        /// tracelistener and creates a list for telemetry subscribers.
        /// </summary>
        public ServerSessionManager()
        {
            TraceManager traceManager = new();

            moduleIdentifier = "Dashboard";
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            _ = new ServerBoardStateManager();
            _ = new ScreenShareServer();
            _contentServer = ContentServerFactory.GetInstance();

            userCount = 0;

            _communicator = CommunicationFactory.GetCommunicator(false);
            _communicator.Subscribe(moduleIdentifier, this);
        }

        /// <summary>
        /// Constructor for the ServerSessionManager, calls the 
        /// tracelistener and creates a list for telemetry subscribers.
        /// </summary>
        public ServerSessionManager(ICommunicator communicator, IContentServer contentServer)
        {
            _contentServer = contentServer;
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            TraceManager traceManager = new();
            traceManager.TraceListener();

            userCount = 0;
            moduleIdentifier = "serverSessionManager";

            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
        }

        /// <summary>
        /// Adds a user to the list of users present in the session
        /// </summary>
        /// <param name="user"> An object of type UserData </param>
        private void AddUserToSession(UserData user)
        {
            lock (this)
            {
                _sessionData.users.Add(user);
            }
        }

        /// <summary>
        /// This function updates the session, notifies telemetry and 
        /// broadcast the new session data
        /// </summary>
        /// <param name="arrivedClient"></param>
        private void ClientArrivalProcedure(ClientToServerData arrivedClient)
        {
            // create a new user and add it to the session. 
            UserData user = CreateUser(arrivedClient.username);
            AddUserToSession(user);

            // sending the all the messages to the new user
            _contentServer.SSendAllMessagesToClient(user.userID);

            // Notify Telemetry about the change in the session object.
            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("addClient", _sessionData, null, user);
        }

        /// <summary>
        /// Creates a new user based on the data arrived from the
        /// client side.
        /// </summary>
        /// <param name="username"> The username of the user </param>
        /// <returns></returns>
        private UserData CreateUser(string username)
        {
            UserData user = new(username, userCount);
            return user;
        }

        /// <summary>
        /// Used to create a summary by fetching all the chats from the 
        /// content moudule and then calling the summary module to create a summary
        /// </summary>
        /// <returns> A SummaryData object that contains the summary of all the chats 
        /// sent present in the meeting. </returns>
        private SummaryData CreateSummary()
        {
            try
            {
                //_contentServer.s
                // fetching all the chats from the content module.
                ChatContext[] allChatsTillNow;
                allChatsTillNow = _contentServer.SGetAllMessages().ToArray();

                // creating the summary from the chats
                string summary = _summarizer.GetSummary(allChatsTillNow);

                // returning the summary
                return new SummaryData(summary);
            }
            catch (Exception e)
            {
                Console.WriteLine("No messages received: " + e.Message);
                return null;
            }
        }

        private void EndMeetProcedure(ClientToServerData receivedObject)
        {
            ChatContext[] allChats = _contentServer.SGetAllMessages().ToArray();

            bool summarySaved = _summarizer.SaveSummary(allChats);

            if (summarySaved == true)
            {
                UserData user = new(receivedObject.username, receivedObject.userID);
                SendDataToClient("endMeet", _sessionData, null, user);
            }
            _communicator.Stop();
            // Cannot find telemetry factory yet.
        }

        /// <summary>
        /// Returns the credentials required to 
        /// Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object containing the port and IP address</returns>
        public MeetingCredentials GetPortsAndIPAddress()
        {
            try
            {
                Trace.WriteLine("Fetching IP Address and port from the networking module");
                string meetAddress = _communicator.Start();

                // Debug.Assert(IsValidIPAddress(meetAddress), "IP Address is NOT valid!");
                if (IsValidIPAddress(meetAddress) != true)
                {
                    Trace.WriteLine("IP Address is not valid, returning null");
                    return null;
                }

                Trace.WriteLine("Returning the IP Address to the UX");
                //string ipAddress = meetAddress.Substring(0, meetAddress.IndexOf(':'));
                string ipAddress = meetAddress[0..meetAddress.IndexOf(':')];
                //int port = Convert.ToInt16(meetAddress.Substring(meetAddress.IndexOf(':') + 2));
                int port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);


                return _meetingCredentials = new MeetingCredentials(ipAddress, port);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }

        }

        /// <summary>
        /// This method is called when a request for getting summary reaches the server side.
        /// A summary is created along with a user object (with the ID and the name of the user who requested the summary)
        /// This data is then sent back to the client side.
        /// </summary>
        /// <param name="receivedObject"></param>
        private void GetSummaryProcedure(ClientToServerData receivedObject)
        {
            SummaryData summaryData = CreateSummary();
            UserData user = new(receivedObject.username, receivedObject.userID);

            SendDataToClient("getSummary", null, summaryData, user);
        }

        /// <summary>
        /// Checks if an IPAddress is valid or not.
        /// </summary>
        /// <param name="IPAddress">The input ipaddress</param>
        /// <returns> true: For valid IP Addresses
        /// false: otherwise</returns>
        private static bool IsValidIPAddress(string IPAddress)
        {
            // Check for null string, whitespaces or absence of colon
            if (String.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false)
            {
                return false;
            }

            // Take the part after the colon as the port number and check the range
            string port = IPAddress.Substring(IPAddress.LastIndexOf(':') + 1);
            if (Int32.TryParse(port, out int portNumber))
            {
                if (portNumber < 0 || portNumber > 65535)
                    return false;
            }

            // Take the part before colon as the ip address
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            string[] byteValues = IPAddress.Split('.');

            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4)
            {
                return false;
            }

            // We have 4 bytes in a address
            //byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out byte tempForParsing));
        }

        /// <summary>
        /// All subscribers are notified about the new session by calling the 
        /// OnAnalyticsChanged function
        /// </summary>
        public void NotifyTelemetryModule()
        {
            for (int i = 0; i < _telemetrySubscribers.Count; ++i)
            {
                lock (this)
                {
                    _telemetrySubscribers[i].OnAnalyticsChanged(_sessionData);
                }
            }
        }

        /// <summary>
        /// This function is called by the networking module when a user joins the meeting. The
        /// SocketObject received from the networking module is then passed again but with a unique 
        /// ID to identify the object uniquely. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="socketObject"></param>
        public void OnClientJoined<T>(T socketObject)
        {
            lock (this)
            {
                userCount += 1;
            }
            _communicator.AddClient<T>(userCount.ToString(), socketObject);
        }

        /// <summary>
        /// Networking module calls this function once the data is sent from the client side.
        /// The SerializedObject is the data sent by the client module which is first deserialized
        /// and processed accordingly.
        /// </summary>
        /// <param name="serializedObject">A string that is the serialized representation of the object sent by the client side. </param>
        public void OnDataReceived(string serializedObject)
        {
            // the object is obtained by deserializing the string and handling the cases 
            // based on the 'eventType' field of the deserialized object. 
            ClientToServerData deserializedObj = _serializer.Deserialize<ClientToServerData>(serializedObject);

            switch (deserializedObj.eventType)
            {
                case "addClient":
                    ClientArrivalProcedure(deserializedObj);
                    return;

                case "getSummary":
                    GetSummaryProcedure(deserializedObj);
                    return;

                case "removeClient":
                    RemoveClientProcedure(deserializedObj);
                    return;

                case "endMeet":
                    EndMeetProcedure(deserializedObj);
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        private void RemoveClientProcedure(ClientToServerData receivedObject)
        {
            UserData userToRemove = new(receivedObject.username, receivedObject.userID);
            RemoveUserFromSession(userToRemove);
            NotifyTelemetryModule();
            SendDataToClient("removeClient", _sessionData, null, userToRemove);
        }

        private void RemoveUserFromSession(UserData userToRemove)
        {
            // raise exception if the user is not in the session or the _sessionData is null
            List<UserData> users = _sessionData.users;
            for (int i = 0; i < users.Count; ++i)
            {
                if (users[i].Equals(userToRemove))
                {
                    lock (this)
                    {
                        _sessionData.users.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData, UserData user)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData = new ServerToClientData(eventName, sessionData, summaryData, user);
                string serializedSessionData = _serializer.Serialize<ServerToClientData>(serverToClientData);
                _communicator.Send(serializedSessionData, moduleIdentifier);
            }
        }

        /// <summary>
        /// Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The listener of the subscriber </param>
        public void Subcribe(ITelemetryNotifications listener)
        {
            lock (this)
            {
                _telemetrySubscribers.Add(listener);
            }
        }

        private readonly string moduleIdentifier;
        private readonly ICommunicator _communicator;
        private readonly ISerializer _serializer;
        int userCount;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly SessionData _sessionData;
        private MeetingCredentials _meetingCredentials;
        private readonly ISummarizer _summarizer;
        private readonly IContentServer _contentServer;
    }
}