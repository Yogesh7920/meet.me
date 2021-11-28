/// <author>Irene Casmir</author>
/// <created>26/11/2021</created>

using Client.ViewModel;
using NUnit.Framework;
using static Testing.UX.Authentication.AuthUtils;

namespace Testing.UX.Authentication
{
    [TestFixture]
    public class AuthViewModelUnitTest
    {
        [SetUp]
        public void SetUp()
        {
            _viewModel = new AuthViewModel(new FakeClientSessionManager());
        }

        private AuthViewModel _viewModel;

        [Test]
        public void OnUserLogin_ReturnBool()
        {
            //Assert
            Assert.AreEqual(_viewModel.SendForAuth("192.168.1.1", 123, "Irene"), true);
            Assert.AreEqual(_viewModel.SendForAuth("192 168.1 .1", 123, "Irene"), false);
            Assert.AreEqual(_viewModel.SendForAuth("192.168.1.1", 123, ""), false);
            Assert.AreEqual(_viewModel.SendForAuth(" ", 123, ""), false);
            Assert.AreEqual(_viewModel.SendForAuth("", 123, "Irene"), false);
        }
    }
}