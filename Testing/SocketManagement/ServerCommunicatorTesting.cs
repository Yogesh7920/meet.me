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

        ICommunicator server = new ServerCommunicator();
        string address=null;
        private TcpClient _clientSocket = new TcpClient();
        private Hashtable _clientIdSocket = new Hashtable();

        [SetUp]
        public void Setup()
        {
            address =server.Start();
            Console.WriteLine(address);
        }
        
        [Test, Category("pass")]
        public void ServerStartTest()
        {
            string[] s = address.Split(":");
            IPAddress ip = IPAddress.Parse(s[0]);
            int port = Int32.Parse(s[1]);
            _clientSocket.Connect(ip, port);
            bool  v = _clientSocket.Connected;
            Assert.AreEqual(v, true);
           
        }

        [Test, Category("pass")]
        public void ClientStartTest()
        {
            string[] s = address.Split(":");
            IPAddress ip = IPAddress.Parse(s[0]);
            int port = Int32.Parse(s[1]);
            ICommunicator client = new ClientCommunicator();
            string v =client.Start(s[0],s[1]);
            Assert.AreEqual(v, "1");
            
        }

        [Test, Category("pass")]
        public void FreePortTest()
        {
            string[] s = address.Split(":");
            IPAddress ip = IPAddress.Parse(s[0]);
            int port = Int32.Parse(s[1]);

            //int port =
            //server.FreeTcpPort();
            bool inUse = true;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = false;
                    break;
                }
            }
            
            Assert.AreEqual(inUse,false);

        }
        [Test, Category("pass")]
        public void ServerAddClientTest()
        {
            TcpClient t = new TcpClient();
            server.AddClient("1", t);
            _clientIdSocket["1"]=t;
            bool v = false;
            if (_clientIdSocket.ContainsKey("1"))
            {
                v = true;
            }
            Assert.AreEqual(v, true) ;
        }

    }
}