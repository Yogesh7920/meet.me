/// <author>Sahil J. Chaudhari</author>
/// <created>20/11/2021</created>
/// <modified>24/11/2021</modified>
/// <summary>
/// This file contains Fake communicator which will mimic network's ICommunicator for testing purpose 
/// </summary>

using System.Collections.Generic;
using System.Diagnostics;
using Networking;
using Content;
using System;

namespace Testing.Content
{
    public class FakeCommunicator : ICommunicator
    {
        private string _sendSerializedStr;
        private List<INotificationHandler> _subscribers;
        private bool _isBroadcast;
        private List<string> _receiverIds;

        public FakeCommunicator()
        {
            _sendSerializedStr = "";
            _isBroadcast = false;
            _receiverIds = new List<string>();
            _subscribers = new List<INotificationHandler>();
        }

        public string Start(string serverIp = null, string serverPort = null)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Indicates the joining of a new client to concerned modules.
        /// </summary>
        /// <typeparam name="T">socketObject.</typeparam>
        /// <param name="clientId">Unique ID of thr Client.</param>
        /// <param name="socketObject">socket object associated with the client.</param>
        public void AddClient<T>(string clientId, T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Notifies all concerned modules regarding the removal of the client.
        /// </summary>
        /// <param name="clientId">Unique ID of the client.</param>
        public void RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sends data to the server[Client-Side].
        ///     Broadcasts data to all connected clients[Server-Side].
        /// </summary>
        /// <param name="data">Data to be sent over the network.</param>
        /// <param name="identifier">Module Identifier.</param>
        public void Send(string data, string identifier)
        {
            _sendSerializedStr = "";
            _sendSerializedStr = data;
            _isBroadcast = true;
            _receiverIds = new List<string> { };
        }

        public void Reset()
        {
            _isBroadcast = false;
            _receiverIds = new List<string> { };
        }

        /// <summary>
        ///     Sends the data to one client[Server-Side].
        /// </summary>
        /// <param name="data">Data to be sent over the network.</param>
        /// <param name="identifier">Module Identifier.</param>
        /// <param name="destination">Client ID of the receiver.</param>
        public void Send(string data, string identifier, string destination)
        {
            _sendSerializedStr = "";
            _receiverIds.Add(destination);
            _isBroadcast = false;
            _sendSerializedStr = data;
        }

        public string GetSentData()
        {
            return _sendSerializedStr;
        }

        public List<string> GetRcvIds()
        {
            return _receiverIds;
        }

        public bool GetIsBroadcast()
        {
            bool flag = _isBroadcast;
            Reset();
            return flag;
        }

        /// <summary>
        ///     Provides a subscription to the modules for listening for the data over the network.
        /// </summary>
        /// <param name="identifier">Module Identifier.</param>
        /// <param name="handler">Module implementation of handler; called to notify about an incoming message.</param>
        /// <param name="priority">Priority Number indicating the weight in queue to be given to the module.</param>
        public void Subscribe(string identifier, INotificationHandler handler, int priority = 1)
        {
            _subscribers.Add(handler);
        }

        public void Notify(string data)
        {
            foreach (var subscriber in _subscribers) subscriber.OnDataReceived(data);
        }
    }
}