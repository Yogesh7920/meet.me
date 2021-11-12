using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Networking
{
    public class ReceiveQueueListener
    {
        private readonly IQueue _receieveQueue;
        private readonly Dictionary<string, INotificationHandler> _notificationHandlers;
        private bool _listenRun;

        /// <summary>
        /// ReceiveQueueListener constructor initializes the queue and the notification handler passed.
        /// </summary>
        public ReceiveQueueListener(IQueue queue, Dictionary<string, INotificationHandler> notificationHandlers)
        {
            _receieveQueue = queue;
            _notificationHandlers = notificationHandlers;
        }

        public void Start()
        {
            Thread listener = new Thread(ListenQueue);
            _listenRun = true;
            listener.Start();
        }
        
        public void Stop()
        {
            _listenRun = false;
        }

        /// <summary>
        /// Listens on the receiving queue and calls Notification Handler of the corresponding module.
        /// </summary>
        public void ListenQueue()
        {
            while (!_receieveQueue.IsEmpty() && _listenRun)
            {
                Packet packet = _receieveQueue.Dequeue();
                string data = packet.SerializedData;
                string moduleIdentifier = packet.ModuleIdentifier;

                // If the _notificationHandlers dictionary contains the moduleIdentifier
                if (_notificationHandlers.ContainsKey(moduleIdentifier))
                {
                    INotificationHandler handler = _notificationHandlers[moduleIdentifier];
                    _ = Task.Run(() => { handler.OnDataReceived(data); });
                }
                else
                {
                    throw new Exception("Handler does not exist");
                }
            }
        }
    }
}