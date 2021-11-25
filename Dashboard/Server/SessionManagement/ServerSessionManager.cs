using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using System.Diagnostics;
using Dashboard.Server.Summary;
using Content;
using Whiteboard;
using ScreenSharing;
using Dashboard.Server.Telemetry;

namespace Dashboard.Server.SessionManagement
{
    // Delegate for the MeetingEnded event
    public delegate void NotifyEndMeet();

    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager, INotificationHandler
    {
        /// <summary>
        /// Constructor for the ServerSessionManager. It initialises the 
        /// tracelistener, whiteboard module, content module, screenshare module, 
        /// networking module, summary module, telemetry moudle and creates a list 
        /// for telemetry subscribers. The serverSessionManager also subscribes 
        /// to the communicator for notifications. It maintains the userCount.
        /// </summary>
        public ServerSessionManager()
        {
            TraceManager traceManager = new();

            moduleIdentifier = "Dashboard";
            summarySaved = false;
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

            _telemetry = new Telemetry.Telemetry();
        }

        /// <summary>
        /// This constructor is used to set fake communicator and contentServer for the
        /// puprose of testing and debugging.
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
            summarySaved = false;
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
        /// broadcast the new session data to all users.
        /// </summary>
        /// <param name="arrivedClient"> A ClientToServerData object which contains the details such as the
        /// eventType and the user who wants to join. </param>
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
            SendDataToClient("addClient", _sessionData, null, null, user);
        }

        /// <summary>
        /// Creates a new user based on the data arrived from the
        /// client side.
        /// </summary>
        /// <param name="username"> The username of the user </param>
        /// <returns>An UserData object that contains a unique ID for the username provided. </returns>
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
                // fetching all the chats from the content module.
                ChatContext[] allChatsTillNow;
                allChatsTillNow = _contentServer.SGetAllMessages().ToArray();

                // creating the summary from the chats
                _sessionSummary = _summarizer.GetSummary(allChatsTillNow);

                // returning the summary
                return new SummaryData(_sessionSummary);
            }
            catch(Exception e)
            {
                Trace.WriteLine("Summary Creation Failed: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// This method is called when the host wants to end the meeting. The summary and analytics
        /// of the session is created and stored locally. The UX server is then notified about the end of the 
        /// meet and the client side session manager is also provided with the same information.
        /// </summary>
        /// <param name="receivedObject"> A ClientToServerData object which contains the eventType for ending
        /// the session.</param>
        private void EndMeetProcedure(ClientToServerData receivedObject)
        {
            int tries = 3;
            // User who requested to end the meet.
            UserData user = new(receivedObject.username, receivedObject.userID);
            try
            {
                // n tries are made to save summary and analytics before ending the meet
                while(tries > 0 && summarySaved == false)
                {
                    // Fetching all the chats from the content module
                    ChatContext[] allChats = _contentServer.SGetAllMessages().ToArray();


                    summarySaved = _summarizer.SaveSummary(allChats);
                    _telemetry.SaveAnalytics(allChats);

                    tries--;
                }
                SendDataToClient("endMeet", _sessionData, null, null, user);

            }
            catch (Exception e) 
            {
                // In case of any exception, the meeting is ended without saving the summary.
                // The user is notified about this
                Trace.WriteLine("The summary/analytics could not be saved: ", e.Message);
                SendDataToClient("endMeet", _sessionData, null, null, user);
            }

            // stopping the communicator and notifying UX server about the End Meet event.
            _communicator.Stop();
            MeetingEnded?.Invoke();
        }

        /// <summary>
        /// Fetches the chats from the content moudle and then asks telemetry to generate analytics on it.
        /// The analytics created are then sent to the client side again.
        /// </summary>
        /// <param name="receivedObject">  A ClientToServerData object which contains the eventType for getting analytics
        /// and the user who requested them. </param>
        private void GetAnalyticsProcedure(ClientToServerData receivedObject)
        {
            UserData user = new(receivedObject.username, receivedObject.userID);
            try
            {
                // Fetching the chats and creating analytics on them
                ChatContext[] allChats = _contentServer.SGetAllMessages().ToArray();
                _sessionAnalytics = _telemetry.GetTelemetryAnalytics(allChats);
                SendDataToClient("getAnalytics", null, null, "somethign", user);
            }
            catch (Exception e)
            {
                // In case of a failure, the user is returned a null object
                Trace.WriteLine("Unable to create analytics: " + e.Message);
                SendDataToClient("getAnalytics", null, null, null, user);

            }
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

                // Invalid credentials results in a returnign a null object
                if (IsValidIPAddress(meetAddress) != true)
                {
                    Trace.WriteLine("IP Address is not valid, returning null");
                    return null;
                }

                // For valid IP address, a MeetingCredentials Object is created and returned
                Trace.WriteLine("Returning the IP Address to the UX");
                string ipAddress = meetAddress[0..meetAddress.IndexOf(':')];
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
        /// A getter function to fetch the summary stored in the server side.
        /// </summary>
        /// <returns> Summary in the form of a string. </returns>
        public string GetStoredSummary()
        {
            return _sessionSummary;
        }

        /// <summary>
        /// Fetches the sessionData of the server. Mainly for testing and debugging.
        /// </summary>
        /// <returns> A sessionData object which contains the current users in the meeting.</returns>
        public SessionData GetSessionData()
        {
            return _sessionData;
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
            SendDataToClient("getSummary", null, summaryData, null, user);
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

            // If a null object or username is received, return without further processing.
            if(deserializedObj == null || deserializedObj.username == null)
            {
                Trace.WriteLine("Null object provided by the client.");
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
                    return;

                case "removeClient":
                    RemoveClientProcedure(deserializedObj);
                    return;

                case "endMeet":
                    EndMeetProcedure(deserializedObj);
                    return;

                default:
                    Trace.WriteLine("Incorrect Event type specified");
                    return;
            }
        }

        /// <summary>
        /// Removes the user received (from the ClientToServerData) object from the sessionData and
        /// Notifies telemetry about it. The new session is then broadcasted to all the users.
        /// </summary>
        /// <param name="receivedObject"> A ClientToServerData object which contains the eventType for removing the user
        /// and the user who wants to leave. </param>
        private void RemoveClientProcedure(ClientToServerData receivedObject)
        {
            UserData userToRemove = new(receivedObject.username, receivedObject.userID);
            RemoveUserFromSession(userToRemove);
            NotifyTelemetryModule();
            SendDataToClient("removeClient", _sessionData, null, null, userToRemove);
        }
        
        /// <summary>
        /// Removes the user from the user list in the sessionData.
        /// </summary>
        /// <param name="userToRemove">A UserData object that denotes the used to remove. </param>
        private void RemoveUserFromSession(UserData userToRemove)
        {
            if(_sessionData == null)
            {
                Trace.Write("Session is empty, cannot remove user");
                return;
            }
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

        /// <summary>
        /// Function to send data from Server to client side of the session manager.
        /// </summary>
        /// <param name="eventName">The type of event. </param>
        /// <param name="sessionData">The current session data. </param>
        /// <param name="summaryData">The summary of the session. </param>
        /// <param name="sessionaAnalytics">The analytics of the session.</param>
        /// <param name="user">The user to broadcast/reply. </param>
        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData, string sessionaAnalytics, UserData user)
        //private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData, SessionAnalytics sessionaAnalytics, UserData user)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData = new ServerToClientData(eventName, sessionData, summaryData, sessionaAnalytics, user);
                string serializedSessionData = _serializer.Serialize<ServerToClientData>(serverToClientData);
                _communicator.Send(serializedSessionData, moduleIdentifier);
            }
        }

        /// <summary>
        /// Subscribes to changes in the session object
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

        private readonly string moduleIdentifier;
        private readonly ICommunicator _communicator;
        private readonly ISerializer _serializer;
        int userCount;
        private string _sessionSummary;
        public bool summarySaved;

        private readonly List<ITelemetryNotifications> _telemetrySubscribers;

        private readonly SessionData _sessionData;
        private MeetingCredentials _meetingCredentials;
        private readonly ISummarizer _summarizer;
        private readonly IContentServer _contentServer;
        private SessionAnalytics _sessionAnalytics;
        private ITelemetry _telemetry;

        public event NotifyEndMeet MeetingEnded;
    }
}