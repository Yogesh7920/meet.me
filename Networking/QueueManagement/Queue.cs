/// <author>Alisetti Sai Vamsi</author>
/// <created>13/10/2021</created>
/// <summary>
/// This file contains the implementation of the IQueue interface.
/// </summary>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Networking
{
    public class Queue : IQueue
    {
        private readonly object _lockObj = new();
        private readonly ConcurrentDictionary<string, ConcurrentQueue<Packet>> _multiLevelQueue;
        private readonly ConcurrentDictionary<string, int> _priorityMap;
        private readonly ManualResetEvent _manualResetEvent;
        private int _avoidStateChange;
        private string _currentModuleIdentifier;
        private int _currentQueue;
        private int _currentWeight;
        private List<string> _moduleIdentifiers;
        private int _queueSize;

        /// <summary>
        ///     Queue constructor initializes multilevel queue, priority map dictionaries and also the state of the queue.
        /// </summary>
        public Queue()
        {
            _multiLevelQueue = new ConcurrentDictionary<string, ConcurrentQueue<Packet>>();
            _priorityMap = new ConcurrentDictionary<string, int>();
            _currentQueue = 0;
            _currentWeight = 0;
            _avoidStateChange = 0;
            _queueSize = 0;
            _manualResetEvent = new(false);
            Trace.WriteLine("[Networking] Initializing Queue Module");
        }

        /// <summary>
        ///     Register Module into the multilevel queue.
        /// </summary>
        /// <param name="moduleId">Unique Id for module.</param>
        /// <param name="priority">Priority Number indicating the weight to be given to the module.</param>
        /// <exception cref="Exception">Does not allow duplicate module identifier</exception>
        public void RegisterModule(string moduleId, int priority)
        {
            if (priority <= 0) throw new Exception("[Networking] Priority should be positive integer");

            // Adding <moduleId, Queue> keyValuePair to the _multiLevelQueue dictionary 
            if (!_multiLevelQueue.TryAdd(moduleId, new ConcurrentQueue<Packet>()))
                throw new Exception("[Networking] Adding Queue to MultiLevelQueue Failed!");

            // Adding <moduleId, priority> keyValuePair to the _priorityMap dictionary
            if (!_priorityMap.TryAdd(moduleId, priority))
            {
                _multiLevelQueue.TryRemove(moduleId, out _);
                throw new Exception("[Networking] Priority Map cannot overwrite existing key");
            }

            // Getting the moduleIds in the priority order
            var orderedIdPairs = _priorityMap.OrderByDescending(s => s.Value);
            _moduleIdentifiers = new List<string>();
            foreach (var keyValuePair in orderedIdPairs)
                _moduleIdentifiers.Add(keyValuePair.Key);

            if (_avoidStateChange == 0)
            {
                // Assigning the _currentWeight to the top priority queue's weight
                var moduleIdentifier = _moduleIdentifiers[_currentQueue];
                _currentWeight = _priorityMap[moduleIdentifier];
                _currentModuleIdentifier = moduleIdentifier;
            }
            else
            {
                _currentQueue = _moduleIdentifiers.FindIndex(x => x == _currentModuleIdentifier);
            }
        }

        /// <summary>
        ///     Size of the queue.
        /// </summary>
        /// <returns>The number of packets the queue holds.</returns>
        public int Size()
        {
            return _queueSize;
        }

        /// <summary>
        ///     Dequeues all the elements.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Clear()
        {
            Trace.WriteLine("[Networking] Clearing all packets from the queue");
            lock (_lockObj)
            {
                _queueSize = 0;
            }

            foreach (var keyValuePair in _multiLevelQueue) keyValuePair.Value.Clear();
            _manualResetEvent.Reset();
        }

        /// <summary>
        ///     Enqueues an object of IPacket.
        /// </summary>
        /// <param name="packet">Reference to the packet that has to be enqueued</param>
        public void Enqueue(Packet packet)
        {
            var moduleIdentifier = packet.ModuleIdentifier;

            // Check if the _multiLevelQueue dictionary contains the moduleIdentifier
            if (_multiLevelQueue.ContainsKey(moduleIdentifier))
            {
                _multiLevelQueue[moduleIdentifier].Enqueue(packet);
                lock (_lockObj)
                {
                    _queueSize += 1;
                    if (_queueSize == 1)
                    {
                        _manualResetEvent.Set();
                    }
                }
            }
            else
            {
                throw new Exception("[Networking] Key Error: Packet holds invalid module identifier");
            }

            Trace.WriteLine($"[Networking] Packet from {moduleIdentifier} Enqueued");
        }

        /// <summary>
        ///     Dequeues an item from the queue and returns the item.
        /// </summary>
        /// <returns>Returns the dequeued packet from the queue.</returns>
        /// <exception cref="Exception">Cannot dequeue an empty queue</exception>
        public Packet Dequeue()
        {
            if (IsEmpty()) throw new Exception("[Networking] Cannot Dequeue empty queue");
            FindNext(); // Populates the fields of _currentQueue, _currentWeight corresponding to the next packet

            var moduleIdentifier = _moduleIdentifiers[_currentQueue];
            _multiLevelQueue[moduleIdentifier].TryDequeue(out var packet);
            lock (_lockObj)
            {
                _currentWeight -= 1;
                _avoidStateChange = 1;
                _queueSize -= 1;
                if (_queueSize == 0)
                {
                    _manualResetEvent.Reset();
                }
            }
            Trace.WriteLine($"[Networking] Dequeued Packet for {moduleIdentifier}");
            return packet;
        }

        /// <summary>
        ///     Peeks into the first element of the queue.
        /// </summary>
        /// <returns>Returns the peeked packet from the queue.</returns>
        /// <exception cref="Exception">Cannot peek an empty queue</exception>
        [ExcludeFromCodeCoverage]
        public Packet Peek()
        {
            if (IsEmpty()) throw new Exception("[Networking] Cannot Peek into empty queue");
            FindNext(); // Populates the fields of _currentQueue, _currentWeight corresponding to the next packet

            var moduleIdentifier = _moduleIdentifiers[_currentQueue];
            _multiLevelQueue[moduleIdentifier].TryPeek(out var packet);

            Trace.WriteLine($"[Networking] Peeking into the queue! Found a packet of {moduleIdentifier}");
            return packet;
        }

        /// <summary>
        ///     Checks if the queue is empty or not.
        /// </summary>
        /// <returns>True if queue is empty and false otherwise.</returns>
        public bool IsEmpty()
        {
            return _queueSize == 0;
        }

        /// <summary>
        ///     Sets the _currentQueue, _currentWeight variables corresponding to the next packet to be dequeued/peeked.
        /// </summary>
        private void FindNext()
        {
            while (true)
            {
                var moduleIdentifier = _moduleIdentifiers[_currentQueue];

                // If the current queue completed its round move to the next queue
                if (_currentWeight == 0)
                {
                    _currentQueue = (_currentQueue + 1) % _multiLevelQueue.Count;
                    moduleIdentifier = _moduleIdentifiers[_currentQueue];
                    _currentWeight = _priorityMap[moduleIdentifier];
                    _currentModuleIdentifier = moduleIdentifier;
                    continue;
                }

                if (!_multiLevelQueue[moduleIdentifier].IsEmpty) return;

                // If the next queue has no packets, then search for the first queue that has a packet
                while (_multiLevelQueue[moduleIdentifier].IsEmpty)
                {
                    _currentQueue = (_currentQueue + 1) % _moduleIdentifiers.Count;
                    moduleIdentifier = _moduleIdentifiers[_currentQueue];
                    _currentWeight = _priorityMap[moduleIdentifier];
                    _currentModuleIdentifier = moduleIdentifier;
                }

                break;
            }
        }

        public void WaitForPacket()
        {
            if (_manualResetEvent.SafeWaitHandle.IsClosed)
            {
                return;
            }
            _manualResetEvent.WaitOne();
        }
        
        public void Close()
        {
            _manualResetEvent.Set();
            _manualResetEvent.Dispose();
        }
    }
}