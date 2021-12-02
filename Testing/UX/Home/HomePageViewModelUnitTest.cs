/// <author>P S Harikrishnan</author>
/// <created>26/11/2021</created>

using Client.ViewModel;
using Dashboard;
using NUnit.Framework;
using System.ComponentModel;
using static Testing.UX.Home.HomeUtils;

namespace Testing.UX.Home
{
    [TestFixture]
    class HomePageViewModelUnitTest
    {
        private HomePageViewModel _homePageViewModel;

        [SetUp]
        public void SetUp()
        {
            _homePageViewModel = new HomePageViewModel(new DummyClientSessionManager());
        }
        [Test]
        public void OnClientSessionChanged_UsersCountIsChanged()
        {
            // Arrange
            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("User1", 1);
            UserData sampleUser2 = new UserData("User2", 2);
            UserData sampleUser3 = new UserData("3", 3);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);
            sampleSession.AddUser(sampleUser3);

            // Act
            _homePageViewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_homePageViewModel.users.Count, 3);
        }
        [Test]
        public void OnLeaveClient()
        {
            _homePageViewModel.LeftClient();
        }
        [Test]
        public void OnPropertyChanged_EventShouldBeRaised()
        {
            string samplePropertyName = "";
            _homePageViewModel.UsersListChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                samplePropertyName = e.PropertyName;
            };

            _homePageViewModel.OnPropertyChanged("testing");
            Assert.IsNotNull(samplePropertyName);
            Assert.AreEqual("testing", samplePropertyName);
        }
    }
}
