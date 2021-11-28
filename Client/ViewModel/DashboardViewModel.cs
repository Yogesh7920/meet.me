using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Defaults;
using System.ComponentModel;
using LiveCharts.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;

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
        /// subscribes to Client Session Manager for summary and telemetry updates
        /// </summary>
        public DashboardViewModel()
        {

            _clientSM = SessionManagerFactory.GetClientSessionManager();

            _sessionAnalytics = new SessionAnalytics();
            _sessionAnalytics.chatCountForEachUser = new Dictionary<int, int>();
            _sessionAnalytics.insincereMembers = new List<int>();
            _sessionAnalytics.userCountAtAnyTime = new Dictionary<DateTime, int>();

            _clientSM.SummaryCreated += (latestSummary) => OnSummaryChanged(latestSummary);
            _clientSM.AnalyticsCreated += (latestAnalytics) => OnAnalyticsChanged(latestAnalytics);

            //UpdateVM();
        
            // Default Setup
            _chatSummary = "Refresh to get the latest stats!";
            _usersList = new List<int>() { 0 };
            _messagesCountList = new List<int>() { 0 };
            _usersCountList = new List<int>() { 1 };
            _timestampList = new List<DateTime>() {
                DateTime.Now,
            };

            usersList = new ObservableCollection<string>((IEnumerable<string>)_usersList.ConvertAll(val => new string(val.ToString())));
            messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
            timestampList = new ObservableCollection<string>((IEnumerable<string>)_timestampList.ConvertAll(val => new string(val.ToString("T"))));
            usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());

            messagesCount = _messagesCountList.AsQueryable().Sum();
            participantsCount = _usersList.Count;
            engagementRate = CalculateEngagementRate();

        }

        /// <summary>
        /// To fetch client session manager for unit testing 
        /// </summary>
        /// <returns>Returns Client Session Manager Object</returns>
        public IUXClientSessionManager GetClientSM()
        {
            return _clientSM;
        }

        /// <summary>
        /// To fetch session analytics object for unit testing 
        /// </summary>
        /// <returns>Returns Session Anaytics Object</returns>
        public SessionAnalytics GetSessionAnalytics()
        {
            return _sessionAnalytics;
        }

        /// <summary>
        /// Fetches the latest telemetry data and discussion summary from the session manager 
        /// Keeps the values of Dashboard up-to-date
        /// </summary>
        public void UpdateVM()
        {

            lock (this)
            {
                _clientSM.GetSummary();
                Debug.WriteLine("[UX] Obtained the latest summary");

                _clientSM.GetAnalytics();
                Debug.WriteLine("[UX] Obtained the latest analytics data");


                if (_sessionAnalytics.chatCountForEachUser.Count != 0)
                {
                    _usersList = new List<int>(this._sessionAnalytics.chatCountForEachUser.Keys);
                    _messagesCountList = new List<int>(this._sessionAnalytics.chatCountForEachUser.Values);
                }


                if (_sessionAnalytics.userCountAtAnyTime.Count != 0)
                {
                    _timestampList = new List<DateTime>(this._sessionAnalytics.userCountAtAnyTime.Keys);
                    _usersCountList = new List<int>(this._sessionAnalytics.userCountAtAnyTime.Values);
                }

                if (_sessionAnalytics.insincereMembers.Count != 0)
                {
                    _insincereMembers = _sessionAnalytics.insincereMembers;
                    _recentlyJoined = _insincereMembers.AsQueryable().Sum();
                }

                usersList = new ObservableCollection<string>((IEnumerable<string>)_usersList.ConvertAll(val => new string(val.ToString())));
                messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
                timestampList = new ObservableCollection<string>((IEnumerable<string>)_timestampList.ConvertAll(val => new string(val.ToString("T"))));
                usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());

                messagesCount = _messagesCountList.AsQueryable().Sum();
                participantsCount = _usersList.Count;
                engagementRate = CalculateEngagementRate();

            }
        }

        /// <summary>
        /// Calculates the engagement rate based on number of users 
        /// who are participating in the discussion
        /// The logic is to deduce the fraction of attendees with 
        /// atleast one message sent in the current discussion
        /// </summary>
        /// <returns>String represting the engagement rate</returns>
        private string CalculateEngagementRate()
        {

            //Trace.Assert(participantsCount >= 0);

            if (participantsCount == 0)
            {
                return "0%";
            }
            
            float activeMembers = _messagesCountList.Count(i => i > 0);
            //Debug.WriteLine("Active Members:{0}, Participants: {1}", activeMembers, participantsCount);
            float engagementRate = (activeMembers / participantsCount) * 100;
            //Debug.WriteLine("Engagement Rate: {0}", engagementRate);
            return engagementRate.ToString("0") + "%";
        }

        private void OnSummaryChanged(string latestSummary)
        {
            lock (this)
            {
                _chatSummary = latestSummary;
                OnPropertyChanged(nameof(chatSummary));
            }
        }

        private void OnAnalyticsChanged(SessionAnalytics latestAnalytics)
        {
            lock (this)
            {
                if (latestAnalytics != null)
                    _sessionAnalytics = latestAnalytics;
                else
                    Debug.WriteLine("[UX]: Null Analytics returned");
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;



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
            get { return _engagementRate = CalculateEngagementRate(); }
            set
            {
                _engagementRate = CalculateEngagementRate();
                OnPropertyChanged(nameof(engagementRate));
            }
        }


        private string _chatSummary;
        private List<DateTime> _timestampList;
        private List<int> _usersCountList;
        private List<int> _messagesCountList;
        private List<int> _usersList;
        private List<int> _insincereMembers;

        private int _messagesCount;
        private int _participantsCount;
        private int _recentlyJoined;
        private string _engagementRate;

        private IUXClientSessionManager _clientSM;
        private SessionAnalytics _sessionAnalytics;
    }
}
