using NUnit.Framework;
using Networking;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace Testing
{
    [TestFixture]
    public class CommunicatorTesting
    {
        [SetUp]
        public void Setup()
        {
            Thread.Sleep(1000);
        }

        [Test, Category("pass")]
        public void Server_and_Client_StartTest()
        {
            Thread.Sleep(1000);
            // start the server
            ICommunicator server = CommunicationFactory.GetCommunicator(false, true);
            string address = server.Start();
            string[] s = address.Split(":");

            // client2 connection 
            ICommunicator client2 = CommunicationFactory.GetCommunicator(true, true);
            string c2 = client2.Start(s[0], s[1]);

            // client3 connection 
            ICommunicator client3 = CommunicationFactory.GetCommunicator(true, true);
            string c3 = client3.Start(s[0], s[1]);

            // client4 connection 
            ICommunicator client4 = CommunicationFactory.GetCommunicator(true, true);
            string c4 = client4.Start(s[0], s[1]);

            // stop all clients 
            client2.Stop();
            client3.Stop();
            client4.Stop();

            // stop the server
            server.Stop();
            Assert.AreEqual("1", c2);
            Assert.AreEqual("1", c3);
            Assert.AreEqual("1", c4);
        }
    }
}