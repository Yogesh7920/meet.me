using Client.ViewModel;
using Dashboard;
using NUnit.Framework;
using static Testing.UX.Home.HomeUtils;

namespace Testing.UX.Home
{
    [TestFixture]
    class HomePageViewModelUnitTest
    {
        private HomePageViewModel _HomePageViewModel;

        [SetUp]
        public void SetUp()
        {
            _HomePageViewModel = new HomePageViewModel();
        }
        [Test]
        public void OnClientSessionChanged_UsersAdded()
        {
            // Arrange
            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Hari1", 1);
            UserData sampleUser2 = new UserData("Hari2", 2);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            // Act
            _HomePageViewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_HomePageViewModel.users.Count, 2);
        }
    }
}
