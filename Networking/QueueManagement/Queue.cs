using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Networking
{
    public class Queue: IQueue
    {
        private ConcurrentDictionary<string, ConcurrentQueue<Packet>> _multiLevelQueue;
        private ConcurrentDictionary<string, int> _priorityMap;
        private List<string> _moduleIdentifiers;
        private int _currentQueue;
        private int _currentWeight;

        public Queue()
        {
            _multiLevelQueue = new ConcurrentDictionary<string, ConcurrentQueue<Packet>>();
            _priorityMap = new ConcurrentDictionary<string, int>();
            _currentQueue = 0;
            _currentWeight = 0;
        }

        public void RegisterModule(string moduleId, int priority)
        {
            if (!(_multiLevelQueue.TryAdd(moduleId, new ConcurrentQueue<Packet>())))
            {
                Trace.WriteLine("Adding Queue to MultiLevelQueue Failed!");
                throw new Exception("Adding Queue to MultiLevelQueue Failed!");
            }
            
            if (!(_priorityMap.TryAdd(moduleId, priority)))
            {
                ConcurrentQueue<Packet> queue;
                _multiLevelQueue.TryRemove(moduleId, out queue);
                throw new Exception("Priority Map Error");
            }
            
            IOrderedEnumerable<KeyValuePair<string, int>> orderedIdPairs = _priorityMap.OrderByDescending(s => s.Value);
            _moduleIdentifiers = new List<string>();
            
            foreach (var keyValuePair in orderedIdPairs)
                _moduleIdentifiers.Add(keyValuePair.Key);

            string moduleIdentifier = _moduleIdentifiers[_currentQueue];
            _currentWeight = _priorityMap[moduleIdentifier];
        }
        
        public int Size()
        {
            int totalPackets = 0;
            foreach (var item in _multiLevelQueue)
            {
                totalPackets += item.Value.Count;
            }
            return totalPackets;
        }
        
        public void Clear()
        {
            foreach (var key in _multiLevelQueue)
            {
                while (key.Value.Count > 0)
                {
                    Packet item;
                    if (!(key.Value.TryDequeue(out item)))
                    {
                        throw new Exception("Empty Queue cannot be dequeued");
                    }
                }
            }
        }

        public void Enqueue(Packet item)
        {
            string moduleIdentifier = item.ModuleIdentifier;
            
            if (_multiLevelQueue.ContainsKey(moduleIdentifier))
            {
                _multiLevelQueue[moduleIdentifier].Enqueue(item);
            }
            else
            {
                throw new Exception("Key Error: Packet holds invalid module identifier");
            }
        }

        public Packet Dequeue()
        {
            if (!(IsEmpty()))
            {
                Packet packet;
                FindNext();
                string moduleIdentifier = _moduleIdentifiers[_currentQueue];
                _multiLevelQueue[moduleIdentifier].TryDequeue(out packet);
                _currentWeight -= 1;
                return packet;
            }
            else
            {
                throw new Exception("Cannot Dequeue empty queue");
            }
        }
        
        public Packet Peek()
        {
            if (!(IsEmpty()))
            {
                Packet packet;
                string moduleIdentifier = _moduleIdentifiers[_currentQueue];
                _multiLevelQueue[moduleIdentifier].TryPeek(out packet);
                return packet;
            }
            else
            {
                throw new Exception("Cannot Peek into empty queue");
            }
        }
        
        public bool IsEmpty()
        {
            if (this.Size() == 0) return true;
            return false;
        }

        private void FindNext()
        {
            string moduleIdentifier = _moduleIdentifiers[_currentQueue];
            if (_currentWeight == 0)
            {
                _currentQueue = (_currentQueue + 1) % _multiLevelQueue.Count;
                moduleIdentifier = _moduleIdentifiers[_currentQueue];   
                _currentWeight = _priorityMap[moduleIdentifier];
                FindNext();
            }
            else
            {
                if (_multiLevelQueue[moduleIdentifier].Count == 0)
                {
                    while (_multiLevelQueue[moduleIdentifier].Count == 0)
                    {
                        _currentQueue = (_currentQueue + 1) % _moduleIdentifiers.Count;
                        moduleIdentifier = _moduleIdentifiers[_currentQueue];
                        _currentWeight = _priorityMap[moduleIdentifier];
                    }    
                }
            }
            
        }
    }
}