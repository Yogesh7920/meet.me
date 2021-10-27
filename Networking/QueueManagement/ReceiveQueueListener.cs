using System;
using System.Collections.Generic;

namespace Networking
{
    public class ReceiveQueueListener
    {
        private IQueue _receieveQueue;
        private Dictionary<string, INotificationHandler> _notificationHandlers;

        public ReceiveQueueListener(Dictionary<string , INotificationHandler> notificationHandlers)
        {
            _receieveQueue = new Queue();
            this._notificationHandlers = notificationHandlers;
        }

        public void ListenQueue()
        {
            while (!(_receieveQueue.IsEmpty()))
            {
                Packet Item = _receieveQueue.Dequeue();
                string Data = Item.SerializedData;
                string ModuleIdentifier = Item.ModuleIdentifier;

                if (_notificationHandlers.ContainsKey(ModuleIdentifier))
                {
                    INotificationHandler Handler = _notificationHandlers[ModuleIdentifier];    
                    Handler.OnDataReceived(Data);
                }
                else
                {
                    throw new Exception("Handler does not exist");
                }
            }        
        }
    }
}