using System.Threading;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class CommunicatorTesting
    {
        [Test, Category("pass")]
        public void Server_and_Client_StartTest()
        {
            // start the server
            ICommunicator server = CommunicationFactory.GetCommunicator(false, true);
            string address = server.Start();
            string[] s = address.Split(":");

            // client1 connection 
            ICommunicator client1 = CommunicationFactory.GetCommunicator(true, true);
            string c1 = client1.Start(s[0], s[1]);

            // client2 connection 
            ICommunicator client2 = CommunicationFactory.GetCommunicator(true, true);
            string c2 = client2.Start(s[0], s[1]);

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