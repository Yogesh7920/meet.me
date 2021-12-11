/// <author>Mitul Kataria</author>
/// <created>19/11/2021</created>
/// <summary>
///		This file consists of DashboardViewModel class
///		which handles the display logic of DashboardView.
///		It works as a value converter by exposing 
///		the data objects from Dashboard Server in such a way 
///		that objects are easily managed and presented. 
/// </summary>

using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Client.ViewModel
{
    /// <summary>
    /// DashboardViewModel contains the rendering logic for 
    /// the session analytics obtained from DashboardDataModel
    /// </summary>
    public class DashboardViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructs the intial setup
        /// subscribes to Client Session Manager 
        /// for summary and telemetry updates
        /// </summary>
        public DashboardViewModel()
        {

            _clientSM = SessionManagerFactory.GetClientSessionManager();
            Trace.Assert(_clientSM != null, "[UX] Client session manager is null");

            _sessionAnalytics = new SessionAnalytics();
            _sessionAnalytics.chatCountForEachUser = new Dictionary<int, int>();
            _sessionAnalytics.insincereMembers = new List<int>();
            _sessionAnalytics.userCountAtAnyTime = new Dictionary<DateTime, int>();

            _clientSM.SummaryCreated += (latestSummary) => OnSummaryChanged(latestSummary);
            _clientSM.AnalyticsCreated += (latestAnalytics) => OnAnalyticsChanged(latestAnalytics);

            _sessionData = DashboardSessionData.GetInstance();

            Trace.WriteLine("[UX] Dashboard subscribed to Client Session Manager");

            // Default Setup
            _chatSummary = "Refresh to get the latest stats!";
            _usersList = new List<int>() { 0 };
            _messagesCountList = new List<int>() { 0 };
            _usersCountList = new List<int>() { 1 };
            _timestampList = new List<DateTime>() {
                DateTime.Now,
            };

            messagesCount = _messagesCountList.AsQueryable().Sum();
            participantsCount = _usersList.Count;
            engagementRate = CalculateEngagementRate(_messagesCountList, _usersList);

            usernamesList = new() { "Refresh to get the latest stats!" };
            usersList = new ObservableCollection<string>((IEnumerable<string>)_usersList.ConvertAll(val => new string(val.ToString())));
            messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
            timestampList = new ObservableCollection<string>((IEnumerable<string>)_timestampList.ConvertAll(val => new string(val.ToString("T"))));
            usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());

            Trace.WriteLine("[UX] Initialized Dashboard Analytics");
        }

        public IUXClientSessionManager GetClientSM()
        {
            return _clientSM;
        }

        /// <summary>
        /// To fetch session analytics data for unit testing 
        /// </summary>
        /// <returns>Current Session Anaytics</returns>
        public SessionAnalytics GetSessionAnalytics()
        {
            return _sessionAnalytics;
        }

        /// <summary>
        /// Fetches the latest telemetry data and 
        /// discussion summary from the session manager 
        /// Keeps the values of Dashboard up-to-date
        /// </summary>
        public void UpdateVM()
        {
            lock (this)
            {
                _clientSM.GetSummary();
                Trace.WriteLine("[UX] Obtained the latest summary");

                _clientSM.GetAnalytics();
                Trace.WriteLine("[UX] Obtained the latest analytics data");

                if (_sessionAnalytics.chatCountForEachUser.Count > 0)
                {
                    _usersList = new List<int>(this._sessionAnalytics.chatCountForEachUser.Keys);
                    _messagesCountList = new List<int>(this._sessionAnalytics.chatCountForEachUser.Values);
                }

                if (_sessionAnalytics.userCountAtAnyTime.Count > 0)
                {
                    _timestampList = new List<DateTime>(this._sessionAnalytics.userCountAtAnyTime.Keys);
                    _usersCountList = new List<int>(this._sessionAnalytics.userCountAtAnyTime.Values);
                }

                _latestSessionData = _sessionData.UpdateSessionData(_latestSessionData);

                _insincereMembers = _sessionAnalytics.insincereMembers;
                messagesCount = _messagesCountList.AsQueryable().Sum();
                participantsCount = _latestSessionData.users.Count;
                engagementRate = CalculateEngagementRate(_usersList, _messagesCountList);

                UpdateUserNames(_latestSessionData, usernamesList);

                //usersList = new ObservableCollection<string>((IEnumerable<string>)_usersList.ConvertAll(val => new string(val.ToString())));
                usersList = usernamesList;
                messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
                timestampList = new ObservableCollection<string>((IEnumerable<string>)_timestampList.ConvertAll(val => new string(val.ToString("T"))));
                usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
                Trace.WriteLine("[UX] Rendered the latest analytics data");
            }
        }

        /// <summary>
        /// Calculates the engagement rate based on number of users 
        /// who are participating in the discussion
        /// The logic is to deduce the fraction of attendees with 
        /// atleast one message sent in the current discussion
        /// </summary>
        /// <param name="usersList">List of users in the session</param>
        /// <param name="messagesCountList">List of messages count per user</param>
        /// <returns>String represting the engagement rate</returns>
        public string CalculateEngagementRate(List<int> usersList, List<int> messagesCountList)
        {

            if (usersList == null || messagesCountList == null)
            {
                return "0%";
            }

            if (usersList.Count == 0)
            {
                return "0%";
            }

            Trace.Assert(usersList != null, "[UX] Null parameter usersList in CalculateEngagementRate");
            Trace.Assert(messagesCountList != null, "[UX] Null parameter messagesCountList in CalculateEngagementRate");

            if (_latestSessionData != null)
            {
                float activeMembers = getActiveUsers(_latestSessionData.users, _sessionAnalytics.chatCountForEachUser);
                float engagementRate = (float)(activeMembers / _latestSessionData.users.Count) * 100;
                return engagementRate.ToString("0") + "%";
            }

            return "0%";
        }

        private int getActiveUsers(List<UserData> currentUsers, Dictionary<int, int> userMsgDict)
        {
            int activeCount = 0;
            foreach (UserData user in currentUsers)
            {
                if (userMsgDict.ContainsKey(user.userID) && userMsgDict[user.userID] != 0)
                {
                    activeCount++;
                }
            }
            return activeCount;
        }

        /// <summary>
        /// Updates chat summary with 
        /// the latest obtained summary from session manager
        /// </summary>
        /// <param name="latestSummary">Latest Summary Value</param>
        public void OnSummaryChanged(string latestSummary)
        {
            lock (this)
            {
                _chatSummary = latestSummary;
                OnPropertyChanged(nameof(chatSummary));
            }
        }

        /// <summary>
        /// Updates the analytics values with the
        /// latest obtained analytics from session manager
        /// </summary>
        /// <param name="latestAnalytics">Latest Analytics Value</param>
        public void OnAnalyticsChanged(SessionAnalytics latestAnalytics)
        {
            lock (this)
            {
                if (latestAnalytics != null)
                    _sessionAnalytics = latestAnalytics;
                else
                    Trace.WriteLine("[UX] Received Null session analytics object");
            }
        }

        private void UpdateUserNames(SessionData session, ObservableCollection<string> usernamesList)
        {
            if (session != null)
            {
                usernamesList.Clear();
                bool found = false;
             
                foreach (int key in _sessionAnalytics.chatCountForEachUser.Keys)
                {
                    foreach (UserData user in _latestSessionData.users)
                    {

                        if (key == user.userID)
                        {
                            this.usernamesList.Add(user.username + "(ID:" + key.ToString() + ")");
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        this.usernamesList.Add("Left User with ID: " + key.ToString());
                    }
                    found = false;
                }
            }
            Trace.WriteLine("[UX] Updated the users list");
        }

        /// <summary>
        /// Notifies view when property value changes
        /// </summary>
        /// <param name="property">Target Property</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Users count list
        /// </summary>
        public ChartValues<ObservableValue> usersCountList { get; private set; }

        /// <summary>
        /// List of timestamps
        /// </summary>
        public ObservableCollection<string> timestampList { get; private set; }

        /// <summary>
        /// Messages count per user following the order as in usersList
        /// </summary>
        public ChartValues<ObservableValue> messagesCountList { get; private set; }

        /// <summary>
        /// List of users present in the meeting
        /// </summary>
        //public ObservableCollection<string> usersList { get; private set; }
        public ObservableCollection<string> usersList { get; private set; }

        /// <summary>
        /// The summary of discussion done in the meeting so far
        /// </summary>
        public string chatSummary
        {
            get { return _chatSummary; }
            set
            {
                _chatSummary = value;
                OnPropertyChanged(nameof(chatSummary));
            }
        }

        /// <summary>
        /// Total number of messages sent in chat during the session
        /// </summary>
        public int messagesCount
        {
            get { return _messagesCount; }
            set
            {
                if (_messagesCount != value)
                {
                    _messagesCount = value;
                    OnPropertyChanged(nameof(messagesCount));
                }
            }
        }

        /// <summary>
        /// Total number of participants in the meeting
        /// </summary>
        public int participantsCount
        {
            get { return _participantsCount; }
            set
            {
                if (_participantsCount != value)
                {
                    _participantsCount = value;
                    OnPropertyChanged(nameof(participantsCount));
                    OnPropertyChanged(nameof(engagementRate));
                }
            }
        }

        /// <summary>
        /// Denotes the percentage of meeting attendees taking part in the discussion
        /// </summary>
        public string engagementRate
        {
            get { return CalculateEngagementRate(_messagesCountList, _usersList); }
            private set
            {
                OnPropertyChanged(nameof(engagementRate));
            }
        }

        public ObservableCollection<string> usernamesList { get; private set; }

        private SessionData _latestSessionData;
        private DashboardSessionData _sessionData;
        private string _chatSummary;
        private List<DateTime> _timestampList;
        private List<int> _usersCountList;
        private List<int> _messagesCountList;
        private List<int> _usersList;
        private List<int> _insincereMembers;

        private int _messagesCount;
        private int _participantsCount;

        private IUXClientSessionManager _clientSM;
        private SessionAnalytics _sessionAnalytics;
    }
}