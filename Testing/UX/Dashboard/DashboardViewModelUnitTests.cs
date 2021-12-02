/// <author>Mitul Kataria</author>
/// <created>27/11/2021</created>
/// <summary>
///		This is the unit testing file
///		for the Dashboard UX module which
///		validates that each unit of the code performs as expected.
/// </summary>

using Client.ViewModel;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Telemetry;
using NUnit.Framework;

namespace Testing.UX.Dashboard
{
    [TestFixture]
    public class DashboardViewModelUnitTests
    {
        [SetUp]
        public void SetUp()
        {
            _viewModel = new DashboardViewModel();
            _clientSM = _viewModel.GetClientSM();
            _sessionAnalytics = _viewModel.GetSessionAnalytics();
        }

        private DashboardViewModel _viewModel;
        private IUXClientSessionManager _clientSM;
        private SessionAnalytics _sessionAnalytics;


        [Test]
        public void SetupTesting()
        {
            Assert.NotNull(_viewModel);
            Assert.NotNull(_clientSM);
            Assert.NotNull(_sessionAnalytics);
        }

        [Test]
        public void TestingInitializations()
        {
            Assert.AreEqual(_viewModel.chatSummary, "Refresh to get the latest stats!");
            Assert.AreEqual(_viewModel.usersList.Count, 1);
            Assert.AreEqual(_viewModel.messagesCountList.Count, 1);
            Assert.AreEqual(_viewModel.timestampList.Count, 1);
            Assert.AreEqual(_viewModel.usersCountList.Count, 1);

            Assert.AreEqual(_viewModel.messagesCount, 0);
            Assert.AreEqual(_viewModel.participantsCount, 1);
            Assert.AreEqual(_viewModel.engagementRate, "0%");
        }
    }
}