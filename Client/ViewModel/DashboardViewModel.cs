using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using System.ComponentModel;
using LiveCharts.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dashboard;
using Dashboard.Client.SessionManagement;
using System.Windows;
using System.Windows.Threading;

namespace Client.ViewModel
{
    /// <summary>
    /// DashboardViewModel contains the rendering logic for 
    /// the session analytics obtained from DashboardDataModel
    /// </summary>
    public class DashboardViewModel : INotifyPropertyChanged
    {

        private IUXClientSessionManager _clientSM;

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
        /// Users count list: : Observable INTS
        /// </summary>
        public ChartValues<ObservableValue> usersCountList { get; private set; }

        /// <summary>
        /// List of timestamps
        /// </summary>
        public ChartValues<ObservableValue> timestampList { get; private set; }

        /// <summary>
        /// Messages count per user following the order as in usersList : Observable INTS
        /// </summary>
        public ChartValues<ObservableValue> messagesCountList { get; private set; }

        /// <summary>
        /// List of users present in the meeting
        /// </summary>
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
        private List<int> _timestampList;
        private List<int> _usersCountList;

        private List<int> _messagesCountList;
        private List<string> _usersList;

        private int _messagesCount;
        private int _participantsCount;
        private string _engagementRate;

        //public DashboardViewModel(string chatSummary, List<int> messages, List<string> users, List<int> usersCount, List<int> time)
        //{
        //    _chatSummary = chatSummary;
        //    _messagesCountList = messages;
        //    _usersList = users;
        //    _usersCountList = usersCount;
        //    _timestampList = time;

        //    usersList = new ObservableCollection<string> { "Mitul", "Ram", "Yogesh", "Rashwanth", "Vishal", "Shreejith" };
        //    messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
        //    usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
        //    timestampList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_timestampList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
        //    participantsCount = _usersList.Count;
        //    messagesCount = _messagesCountList.AsQueryable().Count();
        //    engagementRate = CalculateEngagementRate();
        //}


        private void OnSummaryChange(string latestSummary)
        {
            lock (this)
            {
                chatSummary = latestSummary;
            }
        }

        /// <summary>
        /// Populates the dashboard analytics with random values for the time being
        /// </summary>
        public DashboardViewModel()
        {

            _clientSM = SessionManagerFactory.GetClientSessionManager();
            _clientSM.SummaryCreated += (latestSummary)=>OnSummaryChange(latestSummary);

            _chatSummary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "Sit amet commodo nulla facilisi nullam vehicula ipsum a.Velit sed ullamcorper morbi tincidunt ornare massa eget. Nunc sed augue lacus viverra.";

            usersList = new ObservableCollection<string> { "Mitul", "Ram", "Yogesh", "Rashwanth", "Vishal", "Shreejith" };
            _messagesCountList = new List<int>() { 0, 20, 0, 5, 1, 30 };

            _usersCountList = new List<int>() { 0, 5, 10, 7, 2, 10, 6, 9 };
            _timestampList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

            messagesCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_messagesCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
            usersCountList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_usersCountList.ConvertAll(x => new ObservableValue(x)).AsChartValues());
            timestampList = new ChartValues<ObservableValue>((IEnumerable<ObservableValue>)_timestampList.ConvertAll(x => new ObservableValue(x)).AsChartValues());

            messagesCount = _messagesCountList.AsQueryable().Sum();
            participantsCount = usersList.Count;
            engagementRate = CalculateEngagementRate();

        }

        /// <summary>
        /// Fetches the latest telemetry data and discussion summary from the session manager 
        /// Keeps the values of Dashboard up-to-date
        /// </summary>
        public void UpdateVM()
        {

            // Populating with random values for the time being
            Random r = new();
            int i = 5;
            this.usersCountList.Add(new ObservableValue(r.Next(5, 15)));
            this.timestampList.Add(new ObservableValue(i++));

            this.usersList.Add("Max");
            this.messagesCountList.Add(new ObservableValue(r.Next(10, 30)));

            this.chatSummary += "New String Added";

            this.participantsCount += 1;
            this.messagesCount += 2;
            this.engagementRate = CalculateEngagementRate();

            _clientSM.SummaryCreated += (latestSummary) => OnSummaryChange(latestSummary);

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

            Trace.Assert(participantsCount >= 0);

            if (participantsCount == 0)
            {
                return "0%";
            }

            float activeMembers = 0;

            for (int j = 0; j < messagesCountList.Count; j++)
            {
                if (messagesCountList[j].Value > 0)
                {
                    activeMembers++;
                }
            }
            //Debug.WriteLine("Active Members:{0}, Participants: {1}", activeMembers, participantsCount);

            float engagementRate = (activeMembers / participantsCount) * 100;
            //Debug.WriteLine("Engagement Rate: {0}", engagementRate);
            return engagementRate.ToString("0") + "%";
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
