/// <author>Tausif Iqbal</author>
/// <created>1/11/2021</created>
/// <summary>
///     This file covers the unit tests
///     for client joining the server and leaving the server.
/// </summary>
///

using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class CommunicatorTesting
    {
        [Test]
        [Category("pass")]
        public void Start_ClientServerStartup_StartupMustPass()
        {
            // start the server
            var server = CommunicationFactory.GetCommunicator(false, true);
            var address = server.Start();
            var s = address.Split(":");

            // client1 connection 
            var client1 = CommunicationFactory.GetCommunicator(true, true);
            var c1 = client1.Start(s[0], s[1]);

            // client2 connection 
            var client2 = CommunicationFactory.GetCommunicator(true, true);
            var c2 = client2.Start(s[0], s[1]);

            // stop all clients 
            client2.Stop();
            client1.Stop();

            // stop the server
            server.Stop();
            Assert.AreEqual("1", c2);
            Assert.AreEqual("1", c1);
        }
    }
}