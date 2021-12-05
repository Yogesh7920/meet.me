/// <author>Alisetti Sai Vamsi</author>
/// <created>27/10/2021</created>
/// <summary>
/// This file contains the class definition of ReceiveQueueListener.
/// </summary>

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Networking
{
    public class ReceiveQueueListener
    {
        private readonly Dictionary<string, INotificationHandler> _notificationHandlers;
        private readonly IQueue _receiveQueue;
        private bool _listenRun;

        /// <summary>
        ///     ReceiveQueueListener constructor initializes the queue and the notification handler passed.
        /// </summary>
        public ReceiveQueueListener(IQueue queue, Dictionary<string, INotificationHandler> notificationHandlers)
        {
            _receiveQueue = queue;
            _notificationHandlers = notificationHandlers;
        }

        /// <summary>
        ///     Starts the ReceiveQueueListener on a new thread
        /// </summary>
        public void Start()
        {
            var listener = new Thread(ListenQueue);
            _listenRun = true;
            listener.Start();
        }

        /// <summary>
        ///     Stops the ReceiveQueueListener thread
        /// </summary>
        public void Stop()
        {
            _listenRun = false;
        }

        /// <summary>
        ///     Listens on the receiving queue and calls Notification Handler of the corresponding module.
        /// </summary>
        private void ListenQueue()
        {
            while (_listenRun)
            {
                _receiveQueue.WaitForPacket();
                while (!_receiveQueue.IsEmpty())
                {
                    var packet = _receiveQueue.Dequeue();
                    var data = packet.SerializedData;
                    var moduleIdentifier = packet.ModuleIdentifier;

                    // If the _notificationHandlers dictionary contains the moduleIdentifier
                    if (_notificationHandlers.ContainsKey(moduleIdentifier))
                    {
                        var handler = _notificationHandlers[moduleIdentifier];
                        _ = Task.Run(() =>
                        {
                            handler.OnDataReceived(data);
                            Trace.WriteLine($"[Networking] OnDataReceived notification sent to {moduleIdentifier}");
                        });
                    }
                    else
                    {
                        Trace.WriteLine($"[Networking] Handler for {moduleIdentifier} does not exist");
                    }
                }
            }
        }
    }
}