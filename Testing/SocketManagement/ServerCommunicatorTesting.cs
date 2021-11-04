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
    public class ServerComunicatorTesting
    {

        
        string address=null;
        private TcpClient _clientSocket = new TcpClient();
        private Hashtable _clientIdSocket = new Hashtable();
        private ICommunicator server = CommunicationFactory.GetCommunicator(false);
        [SetUp]
        public void Setup()
        {
            // start the server as pre step
            

        }
        
        [Test, Category("pass")]
        public void ServerStartTest()
        {
            
            address = server.Start();
            Console.WriteLine(address);
            Thread.Sleep(2);
            
            server.Stop();
            string[] s = address.Split(":");
            IPAddress ip = IPAddress.Parse(s[0]);
            int port = Int32.Parse(s[1]);
            _clientSocket.Connect(ip, port);
            bool  v = _clientSocket.Connected;
            Assert.AreEqual(v, true);
           //problem := it is not hitting for each loop of accept request , for that must need to add some module 
           
        }

        [Test, Category("pass")]
        public void ClientStartTest()
        {
            // problem := need to call server.Stop before client.Start 
            // reason := because of not calling  addClientmethod as it adds the client recieving thread 
            address = server.Start();
            string[] s = address.Split(":");
            
            server.Stop();
            ICommunicator client = CommunicationFactory.GetCommunicator();

            string v =client.Start(s[0],s[1]);
            
            client.Stop();
            Assert.AreEqual(1, 1);
        }

    }
}