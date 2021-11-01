using System;
using System.Collections.Generic;

namespace Networking
{
    public class ReceiveQueueListener
    {
        private IQueue _receieveQueue;
        private Dictionary<string, INotificationHandler> _notificationHandlers;

        public ReceiveQueueListener(IQueue queue, Dictionary<string , INotificationHandler> notificationHandlers)
        {
            _receieveQueue = queue;
            _notificationHandlers = notificationHandlers;
        }

        public void ListenQueue()
        {
            while (!(_receieveQueue.IsEmpty()))
            {
                Packet packet = _receieveQueue.Dequeue();
                string data = packet.SerializedData;
                string moduleIdentifier = packet.ModuleIdentifier;

                if (_notificationHandlers.ContainsKey(moduleIdentifier))
                {
                    INotificationHandler handler = _notificationHandlers[moduleIdentifier];    
                    handler.OnDataReceived(data);
                }
                else
                {
                    throw new Exception("Handler does not exist");
                }
            }        
        }
    }
}