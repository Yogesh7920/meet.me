using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Content;
using Dashboard.Server.Summary;
using Networking;

namespace Dashboard.Server.SessionManagement
{
    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        private readonly ICommunicator _communicator;
        private readonly IContentServer _contentServer;
        private readonly ISerializer _serializer;

        private readonly SessionData _sessionData;
        private readonly ISummarizer _summarizer;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly string moduleIdentifier;
        private MeetingCredentials _meetingCredentials;
        private int userCount;

        /// <summary>
        ///     Constructor for the ServerSessionManager, calls the
        ///     tracelistener and creates a list for telemetry subscribers.
        /// </summary>
        public ServerSessionManager()
        {
            _contentServer = ContentServerFactory.GetInstance();
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            TraceManager traceManager = new();
            traceManager.TraceListener();

            userCount = 0;
            moduleIdentifier = "serverSessionManager";

            _communicator = CommunicationFactory.GetCommunicator(false);
            _communicator.Subscribe(moduleIdentifier, this);
        }

        /// <summary>
        ///     Constructor for the ServerSessionManager, calls the
        ///     tracelistener and creates a list for telemetry subscribers.
        /// </summary>
        public ServerSessionManager(ICommunicator communicator)
        {
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();

            TraceManager traceManager = new();
            traceManager.TraceListener();

            userCount = 0;
            moduleIdentifier = "serverSessionManager";

            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
        }

        /// <summary>
        ///     This function is called by the networking module when a user joins the meeting. The
        ///     SocketObject received from the networking module is then passed again but with a unique
        ///     ID to identify the object uniquely.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="socketObject"></param>
        public void OnClientJoined<T>(T socketObject)
        {
            lock (this)
            {
                userCount += 1;
            }

            _communicator.AddClient(userCount.ToString(), socketObject);
        }

        /// <summary>
        ///     Networking module calls this function once the data is sent from the client side.
        ///     The SerializedObject is the data sent by the client module which is first deserialized
        ///     and processed accordingly.
        /// </summary>
        /// <param name="serializedObject">A string that is the serialized representation of the object sent by the client side. </param>
        public void OnDataReceived(string serializedObject)
        {
            // the object is obtained by deserializing the string and handling the cases 
            // based on the 'eventType' field of the deserialized object. 
            var deserializedObj = _serializer.Deserialize<ClientToServerData>(serializedObject);

            switch (deserializedObj.eventType)
            {
                case "addClient":
                    ClientArrivalProcedure(deserializedObj);
                    return;

                case "getSummary":
                    GetSummaryProcedure(deserializedObj);
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Returns the credentials required to
        ///     Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object containing the port and IP address</returns>
        public MeetingCredentials GetPortsAndIPAddress()
        {
            Trace.WriteLine("Fetching IP Address and port from the networking module");
            var meetAddress = _communicator.Start();

            if (IsValidIPAddress(meetAddress) != true)
            {
                Trace.WriteLine("IP Address is not valid, returning null");
                return null;
            }

            Trace.WriteLine("Returning the IP Address to the UX");
            //string ipAddress = meetAddress.Substring(0, meetAddress.IndexOf(':'));
            var ipAddress = meetAddress[..meetAddress.IndexOf(':')];
            //int port = Convert.ToInt16(meetAddress.Substring(meetAddress.IndexOf(':') + 2));
            var port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);


            return _meetingCredentials = new MeetingCredentials(ipAddress, port);
        }

        /// <summary>
        ///     Adds a user to the list of users present in the session
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
        ///     This function updates the session, notifies telemetry and
        ///     broadcast the new session data
        /// </summary>
        /// <param name="arrivedClient"></param>
        private void ClientArrivalProcedure(ClientToServerData arrivedClient)
        {
            // create a new user and add it to the session. 
            var user = CreateUser(arrivedClient.username);
            AddUserToSession(user);

            // sending the all the messages to the new user
            _contentServer.SSendAllMessagesToClient(user.userID);

            // Notify Telemetry about the change in the session object.
            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("addClient", _sessionData, user);
        }

        /// <summary>
        ///     Creates a new user based on the data arrived from the
        ///     client side.
        /// </summary>
        /// <param name="username"> The username of the user </param>
        /// <returns></returns>
        private UserData CreateUser(string username)
        {
            UserData user = new(username, userCount);
            return user;
        }

        /// <summary>
        ///     Used to create a summary by fetching all the chats from the
        ///     content moudule and then calling the summary module to create a summary
        /// </summary>
        /// <returns>
        ///     A SummaryData object that contains the summary of all the chats
        ///     sent present in the meeting.
        /// </returns>
        private SummaryData CreateSummary()
        {
            // this double is reprent the ratio of summary size to the original content size.
            // the plans are to take this input from the UX, it will be changed accordingly
            var amountOfSummary = 0.6;

            // fetching all the chats from the content module.
            var allChatsTillNow = _contentServer.SGetAllMessages().ToArray();

            // creating the summary from the chats
            var summary = _summarizer.GetSummary(allChatsTillNow, amountOfSummary);

            // returning the summary
            return new SummaryData(summary);
        }

        /// <summary>
        ///     This method is called when a request for getting summary reaches the server side.
        ///     A summary is created along with a user object (with the ID and the name of the user who requested the summary)
        ///     This data is then sent back to the client side.
        /// </summary>
        /// <param name="receivedObject"></param>
        private void GetSummaryProcedure(ClientToServerData receivedObject)
        {
            var summaryData = CreateSummary();
            var user = new UserData(receivedObject.username, receivedObject.userID);

            SendDataToClient("getSummary", summaryData, user);
        }

        /// <summary>
        ///     Checks if an IPAddress is valid or not.
        /// </summary>
        /// <param name="IPAddress">The input ipaddress</param>
        /// <returns>
        ///     true: For valid IP Addresses
        ///     false: otherwise
        /// </returns>
        private static bool IsValidIPAddress(string IPAddress)
        {
            // Check for null string, whitespaces or absence of colon
            if (string.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false) return false;

            // Take the part before colon as the ip address
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            var byteValues = IPAddress.Split('.');

            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4) return false;

            // We have 4 bytes in a address
            //byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out var tempForParsing));
        }

        /// <summary>
        ///     All subscribers are notified about the new session by calling the
        ///     OnAnalyticsChanged function
        /// </summary>
        public void NotifyTelemetryModule()
        {
            for (var i = 0; i < _telemetrySubscribers.Count; ++i)
                lock (this)
                {
                    _telemetrySubscribers[i].OnAnalyticsChanged(_sessionData);
                }
        }

        private void SendDataToClient(string eventName, IRecievedFromServer objectToSend, UserData user)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData = new ServerToClientData(eventName, objectToSend, user);
            }

            var serializedSessionData = _serializer.Serialize(serverToClientData);
            _communicator.Send(serializedSessionData, moduleIdentifier);
        }

        /// <summary>
        ///     Subscribes to changes in the session object
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
    }
}