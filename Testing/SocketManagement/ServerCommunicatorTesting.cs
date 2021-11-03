using NUnit.Framework;
using Networking;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System;
using System.Net.NetworkInformation;

namespace Testing
{
    [TestFixture]
    public class ServerComunicatorTesting
    {

        
        string address=null;
        private TcpClient _clientSocket = new TcpClient();
        private Hashtable _clientIdSocket = new Hashtable();
        ICommunicator server = CommunicationFactory.GetCommunicator(false);
        [SetUp]
        public void Setup()
        {
            // start serve as pre step
            address = server.Start();
            Console.WriteLine(address);
        }
        
        [Test, Category("pass")]
        public void ServerStartTest()
        {
            
            Console.WriteLine(address);
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
            //problem := need to stop server before calling client.Start 
            // need to check the client stop part by passing some random port
            server.Stop();
            Console.WriteLine(address);
            string[] s = address.Split(":");
            ICommunicator client = CommunicationFactory.GetCommunicator();
            
            string v =client.Start(s[0],s[1]);
            client.Stop();
            
            Assert.AreEqual(v, "1");
        }


    }
}