/// <author>Vishal Rao</author>
/// <created>28/11/2021</created>
/// <summary>
///     This file contains testcases for HomePage.
/// </summary>

using Client.ViewModel;
using NUnit.Framework;

namespace Testing.E2E.Vishal
{
    [TestFixture]
    public class E2ETestHomePage
    {
        [SetUp]
        public void Setup()
        {
            // Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _viewModel = new HomePageViewModel();
        }

        [OneTimeTearDown]
        public void Close()
        {
            // Environment.SetEnvironmentVariable("TEST_MODE", "false");
        }

        private HomePageViewModel _viewModel;

        [Test]
        public void ClientJoin()
        {
            /*
            // Arrange
            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Vishal", 46);
            UserData sampleUser2 = new UserData("User2", 56);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            // Act
            _viewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.users.Count, 2);
            */
        }
    }
}