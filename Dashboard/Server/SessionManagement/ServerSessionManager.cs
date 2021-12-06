/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the implementation of Server session manager.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Content;
using Dashboard.Server.Summary;
using Dashboard.Server.Telemetry;
using Networking;
using ScreenSharing;
using Whiteboard;

namespace Dashboard.Server.SessionManagement
{
    // Delegate for the MeetingEnded event
    public delegate void NotifyEndMeet();

    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        private readonly ICommunicator _communicator;
        private readonly IContentServer _contentServer;
        private readonly ISerializer _serializer;

        private readonly SessionData _sessionData;
        private readonly ISummarizer _summarizer;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly string moduleIdentifier;
        private readonly bool testmode;
        private MeetingCredentials _meetingCredentials;
        private SessionAnalytics _sessionAnalytics;
        private string _sessionSummary;
        private ITelemetry _telemetry;
        public bool summarySaved;
        private int userCount;

        /// <summary>
        ///     Constructor for the ServerSessionManager. It initialises the
        ///     tracelistener, whiteboard module, content module, screenshare module,
        ///     networking module, summary module, telemetry moudle and creates a list
        ///     for telemetry subscribers. The serverSessionManager also subscribes
        ///     to the communicator for notifications. It maintains the userCount.
        /// </summary>
        public ServerSessionManager()
        {
            _ = new TraceManager();

            Trace.WriteLine("[Server Dashboard] Server Dashboard initialised.");

            moduleIdentifier = "Dashboard";
            summarySaved = false;
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();

            userCount = 0;

            _communicator = CommunicationFactory.GetCommunicator(false);
            Trace.WriteLine("[Server Dashboard] Subscribed to communicator.");
            _communicator.Subscribe(moduleIdentifier, this);

            //_telemetry = new Telemetry.Telemetry();
            if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E") return;
            _ = ServerBoardCommunicator.Instance;
            _ = ScreenShareFactory.GetScreenShareServer();
            _contentServer = ContentServerFactory.GetInstance();
        }

        /// <summary>
        ///     This constructor is used to set fake communicator and contentServer for the
        ///     puprose of testing and debugging.
        /// </summary>
        public ServerSessionManager(ICommunicator communicator, IContentServer contentServer)
        {
            _contentServer = contentServer;
            _sessionData = new SessionData();
            _serializer = new Serializer();
            _telemetrySubscribers = new List<ITelemetryNotifications>();
            _summarizer = SummarizerFactory.GetSummarizer();
            //_ = new ScreenShareServer();


            TraceManager traceManager = new();
            traceManager.TraceListener();

            userCount = 0;
            moduleIdentifier = "serverSessionManager";

            _communicator = communicator;
            _communicator.Subscribe(moduleIdentifier, this);
            summarySaved = false;
            testmode = true;
            if (Environment.GetEnvironmentVariable("TEST_MODE") == "E2E") return;
            _ = ScreenShareFactory.GetScreenShareServer();
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
        ///     This method is called by the networking module when the user is disconnected from the meet.
        ///     <param name="userIDString">nhe ID of the leaving user in string format. </param>
        public void OnClientLeft(string userIDString)
        {
            Trace.WriteLine("[Server Dashboard] Client disconnected.");
            var userIDInt = int.Parse(userIDString);
            RemoveClientProcedure(null, userIDInt);
        }

        /// <summary>
        ///     Networking module calls this function once the data is sent from the client side.
        ///     The SerializedObject is the data sent by the client module which is first deserialized
        ///     and processed accordingly.
        /// </summary>
        /// <param name="serializedObject">A string that is the serialized representation of the object sent by the client side. </param>
        public void OnDataReceived(string serializedObject)
        {
            if (serializedObject == null)
            {
                Trace.WriteLine("[Server Dashboard] Null received from client");
                return;
            }

            // the object is obtained by deserializing the string and handling the cases 
            // based on the 'eventType' field of the deserialized object. 
            var deserializedObj = _serializer.Deserialize<ClientToServerData>(serializedObject);

            // If a null object or username is received, return without further processing.
            if (deserializedObj == null || deserializedObj.username == null)
            {
                Trace.WriteLine("[Server Dashboard] Null object provided by the client.");
                return;
            }

            switch (deserializedObj.eventType)
            {
                case "addClient":
                    ClientArrivalProcedure(deserializedObj);
                    return;

                case "getSummary":
                    GetSummaryProcedure(deserializedObj);
                    return;

                case "getAnalytics":
                    GetAnalyticsProcedure(deserializedObj);
                    return;

                case "removeClient":
                    RemoveClientProcedure(deserializedObj);
                    return;

                case "endMeet":
                    EndMeetProcedure(deserializedObj);
                    return;

                default:
                    Trace.WriteLine("[Server Dashboard] Incorrect Event type specified");
                    return;
            }
        }

        /// <summary>
        ///     Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The listener of the subscriber </param>
        public void Subscribe(ITelemetryNotifications listener)
        {
            lock (this)
            {
                _telemetrySubscribers.Add(listener);
            }
        }

        /// <summary>
        ///     Returns the credentials required to
        ///     Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object containing the port and IP address</returns>
        public MeetingCredentials GetPortsAndIPAddress()
        {
            try
            {
                Trace.WriteLine("[Server Dashboard] Fetching IP Address and port from the networking module");
                var meetAddress = _communicator.Start();

                // Invalid credentials results in a returnign a null object
                if (IsValidIPAddress(meetAddress) != true)
                {
                    Trace.WriteLine("[Server Dashboard] IP Address is not valid, returning null");
                    return null;
                }

                // For valid IP address, a MeetingCredentials Object is created and returned
                Trace.WriteLine("[Server Dashboard] Returning the IP Address to the UX");
                var ipAddress = meetAddress[..meetAddress.IndexOf(':')];
                var port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);

                return _meetingCredentials = new MeetingCredentials(ipAddress, port);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
        }

        public event NotifyEndMeet MeetingEnded;

        /// <summary>
        ///     Adds a user to the list of users present in the session
        /// </summary>
        /// <param name="user"> An object of type UserData </param>
        private void AddUserToSession(UserData user)
        {
            lock (this)
            {
                Trace.WriteLine("[Server Dashboard] Client added to the server session.");
                _sessionData.users.Add(user);
            }
        }

        /// <summary>
        ///     This function updates the session, notifies telemetry and
        ///     broadcast the new session data to all users.
        /// </summary>
        /// <param name="arrivedClient">
        ///     A ClientToServerData object which contains the details such as the
        ///     eventType and the user who wants to join.
        /// </param>
        private void ClientArrivalProcedure(ClientToServerData arrivedClient)
        {
            // create a new user and add it to the session. 
            var user = CreateUser(arrivedClient.username);
            AddUserToSession(user);

            // Notify Telemetry about the change in the session object.
            Trace.WriteLine("[Server Dashboard] Notifying Telemetry module about the session changes.");

            NotifyTelemetryModule();

            // serialize and broadcast the data back to the client side.
            SendDataToClient("addClient", _sessionData, null, null, user);
        }

        /// <summary>
        ///     Creates a new user based on the data arrived from the
        ///     client side.
        /// </summary>
        /// <param name="username"> The username of the user </param>
        /// <returns>An UserData object that contains a unique ID for the username provided. </returns>
        private UserData CreateUser(string username)
        {
            if (userCount == 1)
                _telemetry = testmode ? new Telemetry.Telemetry(this) : TelemetryFactory.GetTelemetryInstance();
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
            try
            {
                // fetching all the chats from the content module.
                ChatContext[] allChatsTillNow;
                allChatsTillNow = _contentServer.SGetAllMessages().ToArray();

                // creating the summary from the chats
                _sessionSummary = _summarizer.GetSummary(allChatsTillNow);

                // returning the summary
                Trace.WriteLine("[Server Dashboard] Summary created.");
                return new SummaryData(_sessionSummary);
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Server Dashboard] Summary Creation Failed: " + e.Message);
                return null;
            }
        }

        /// <summary>
        ///     This method is called when the host wants to end the meeting. The summary and analytics
        ///     of the session is created and stored locally. The UX server is then notified about the end of the
        ///     meet and the client side session manager is also provided with the same information.
        /// </summary>
        /// <param name="receivedObject">
        ///     A ClientToServerData object which contains the eventType for ending
        ///     the session.
        /// </param>
        private void EndMeetProcedure(ClientToServerData receivedObject)
        {
            var tries = 3;
            try
            {
                // n tries are made to save summary and analytics before ending the meet
                while (tries > 0 && summarySaved == false)
                {
                    // Fetching all the chats from the content module
                    var allChats = _contentServer.SGetAllMessages().ToArray();

                    summarySaved = _summarizer.SaveSummary(allChats);
                    _telemetry.SaveAnalytics(allChats);

                    tries--;
                }

                SendDataToClient("endMeet", _sessionData, null, null, null);
            }
            catch (Exception e)
            {
                // In case of any exception, the meeting is ended without saving the summary.
                // The user is notified about this
                Trace.WriteLine("[Server Dashboard] The summary/analytics could not be saved: ", e.Message);
                SendDataToClient("endMeet", _sessionData, null, null, null);
            }

            // stopping the communicator and notifying UX server about the End Meet event.
            _communicator.Stop();
            Trace.WriteLine("[Server Dashboard] Notifying server UX about the end of the meet.");
            MeetingEnded?.Invoke();
        }

        /// <summary>
        ///     Fetches the chats from the content moudle and then asks telemetry to generate analytics on it.
        ///     The analytics created are then sent to the client side again.
        /// </summary>
        /// <param name="receivedObject">
        ///     A ClientToServerData object which contains the eventType for getting analytics
        ///     and the user who requested them.
        /// </param>
        private void GetAnalyticsProcedure(ClientToServerData receivedObject)
        {
            UserData user = new(receivedObject.username, receivedObject.userID);
            try
            {
                // Fetching the chats and creating analytics on them
                var allChats = _contentServer.SGetAllMessages().ToArray();
                _sessionAnalytics = _telemetry.GetTelemetryAnalytics(allChats);
                Trace.WriteLine("[Server Dashboard] Got analytics from Telemetry.");
                SendDataToClient("getAnalytics", null, null, _sessionAnalytics, user);
            }
            catch (Exception e)
            {
                // In case of a failure, the user is returned a null object
                Trace.WriteLine("[Server Dashboard] Unable to create analytics: " + e.Message);
                SendDataToClient("getAnalytics", null, null, null, user);
            }
        }

        /// <summary>
        ///     A getter function to fetch the summary stored in the server side.
        /// </summary>
        /// <returns> Summary in the form of a string. </returns>
        public string GetStoredSummary()
        {
            return _sessionSummary;
        }

        /// <summary>
        ///     Fetches the sessionData of the server. Mainly for testing and debugging.
        /// </summary>
        /// <returns> A sessionData object which contains the current users in the meeting.</returns>
        public SessionData GetSessionData()
        {
            return _sessionData;
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
            UserData user = new(receivedObject.username, receivedObject.userID);
            SendDataToClient("getSummary", null, summaryData, null, user);
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

            // Take the part after the colon as the port number and check the range
            var port = IPAddress[(IPAddress.LastIndexOf(':') + 1)..];
            if (int.TryParse(port, out var portNumber))
                if (portNumber < 0 || portNumber > 65535)
                    return false;

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
                    Trace.WriteLine("[Server Dashboard] Notifying Telemetry module about the session data changes.");
                    _telemetrySubscribers[i].OnAnalyticsChanged(_sessionData);
                }
        }

        /// <summary>
        ///     Removes the user received (from the ClientToServerData) object from the sessionData and
        ///     Notifies telemetry about it. The new session is then broadcasted to all the users.
        /// </summary>
        /// <param name="receivedObject">
        ///     A ClientToServerData object which contains the eventType for removing the user
        ///     and the user who wants to leave.
        /// </param>
        private void RemoveClientProcedure(ClientToServerData receivedObject, int userID = -1)
        {
            int userIDToRemove;
            if (userID == -1)
                userIDToRemove = receivedObject.userID;
            else
                userIDToRemove = userID;

            if (_sessionData.users.Count == 1)
            {
                EndMeetProcedure(receivedObject);
                return;
            }

            var removedUser = _sessionData.RemoveUserFromSession(userIDToRemove);
            if (removedUser != null)
            {
                Trace.WriteLine("[Server Dashboard] Removed from session: " + removedUser);
                NotifyTelemetryModule();
                SendDataToClient("removeClient", _sessionData, null, null, removedUser);
            }
        }


        /// <summary>
        ///     Function to send data from Server to client side of the session manager.
        /// </summary>
        /// <param name="eventName">The type of event. </param>
        /// <param name="sessionData">The current session data. </param>
        /// <param name="summaryData">The summary of the session. </param>
        /// <param name="sessionaAnalytics">The analytics of the session.</param>
        /// <param name="user">The user to broadcast/reply. </param>
        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData,
            SessionAnalytics sessionaAnalytics, UserData user)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData =
                    new ServerToClientData(eventName, sessionData, summaryData, sessionaAnalytics, user);
                var serializedSessionData = _serializer.Serialize(serverToClientData);
                Trace.WriteLine("[Server Dashboard] Sending data to the client.");
                _communicator.Send(serializedSessionData, moduleIdentifier);
            }
        }
    }
}