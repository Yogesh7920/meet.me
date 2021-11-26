/**
 * owned by: Irene Casmir
 * created by: Irene Casmir
 * date created: 26/11/2021
 * date modified: 27/11/2021
**/
using NUnit.Framework;
using Client.ViewModel;
using static Testing.UX.Authentication.AuthUtils;

namespace Testing.UX.Authentication
{
    [TestFixture]
    public class AuthViewModelUnitTest
    {
        private AuthViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new AuthViewModel(new FakeClientSessionManager()) ;
        }

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