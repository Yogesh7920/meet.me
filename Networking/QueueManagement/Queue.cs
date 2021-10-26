using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Networking
{
    public class Queue: IQueue
    {
        private ConcurrentQueue<Packet> _screenShareQueue;
        private ConcurrentQueue<Packet> _whiteBoardQueue;
        private ConcurrentQueue<Packet> _fileQueue;
        private ConcurrentQueue<Packet> _chatQueue;
        private ConcurrentDictionary<string, ConcurrentQueue<Packet>> _multiLevelQueue;
        private ConcurrentDictionary<int, Tuple<string, int>> _priorityMap;
        private int _currentQueue;
        private int _currentWeight;

        public Queue()
        {
            _screenShareQueue = new ConcurrentQueue<Packet>();
            _whiteBoardQueue  = new ConcurrentQueue<Packet>();
            _fileQueue  = new ConcurrentQueue<Packet>();
            _chatQueue  = new ConcurrentQueue<Packet>();
            _multiLevelQueue = new ConcurrentDictionary<string, ConcurrentQueue<Packet>>();
            _priorityMap = new ConcurrentDictionary<int, Tuple<string, int>>();
            
            if (!(_multiLevelQueue.TryAdd("S", _screenShareQueue)))
            {
                throw new Exception("Adding ScreenShareQueue to MultiLevelQueue Failed!");
            }
            
            if (!(_multiLevelQueue.TryAdd("W", _whiteBoardQueue)))
            {
                throw new Exception("Adding WhiteBoardQueue to MultiLevelQueue Failed!");
            }
            
            if (!(_multiLevelQueue.TryAdd("F", _fileQueue)))
            {
                throw new Exception("Adding FileQueue to MultiLevelQueue Failed!");
            }
            
            if (!(_multiLevelQueue.TryAdd("C", _chatQueue)))
            {
                throw new Exception("Adding ChatQueue to MultiLevelQueue Failed!");
            }

            try
            {
                _priorityMap[0] = Tuple.Create("S", 4);
                _priorityMap[1] = Tuple.Create("W", 3);
                _priorityMap[2] = Tuple.Create("F", 2);
                _priorityMap[3] = Tuple.Create("C", 1);
                _currentQueue = 0;
                _currentWeight = _priorityMap[_currentQueue].Item2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public int Size()
        {
            int TotalPackets = 0;
            foreach (var item in _multiLevelQueue)
            {
                TotalPackets += item.Value.Count;
            }
            return TotalPackets;
        }
        
        public void Clear()
        {
            foreach (var item in _multiLevelQueue)
            {
                while (item.Value.Count > 0)
                {
                    Packet Item;
                    try
                    {
                        item.Value.TryDequeue(out Item);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        public void Enqueue(Packet item)
        {
            string ModuleIdentifier = item.ModuleIdentifier;
            
            if (_multiLevelQueue.ContainsKey(ModuleIdentifier))
            {
                try
                {
                    _multiLevelQueue[ModuleIdentifier].Enqueue(item);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            else
            {
                throw new Exception("MultiLevelQueue Key Error");
            }
        }
        
        public Packet Dequeue()
        {
            if (!(IsEmpty()))
            {
                Packet Item;
                if (_currentWeight != 0)
                {
                    try
                    {
                        string ModuleIdentifier = _priorityMap[_currentQueue].Item1;
                        if(_multiLevelQueue[ModuleIdentifier].Count != 0)
                            _multiLevelQueue[ModuleIdentifier].TryDequeue(out Item);
                        else
                        {
                            while (_multiLevelQueue[ModuleIdentifier].Count == 0)
                            {
                                _currentQueue = (_currentQueue + 1) % _multiLevelQueue.Count;
                                ModuleIdentifier = _priorityMap[_currentQueue].Item1;
                                _currentWeight = _priorityMap[_currentQueue].Item2;
                            }
                            _multiLevelQueue[ModuleIdentifier].TryDequeue(out Item);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    _currentWeight -= 1;
                }
                else
                {
                    _currentQueue = (_currentQueue + 1) % _multiLevelQueue.Count;
                    _currentWeight = _priorityMap[_currentQueue].Item2;
                    Item = Dequeue();
                }
                
                return Item;
            }
            else
            {
                throw new Exception("Queue is empty");
            }
        }
        
        public Packet Peek()
        {
            if (!(IsEmpty()))
            {
                Packet Item;
                try
                {
                    string ModuleIdentifier = _priorityMap[_currentQueue].Item1;
                    if(_multiLevelQueue[ModuleIdentifier].Count != 0)
                        _multiLevelQueue[ModuleIdentifier].TryPeek(out Item);
                    else
                    {
                        while (_multiLevelQueue[ModuleIdentifier].Count == 0)
                        {
                            _currentQueue = (_currentQueue + 1) % _multiLevelQueue.Count;
                            ModuleIdentifier = _priorityMap[_currentQueue].Item1;
                        }
                        _multiLevelQueue[ModuleIdentifier].TryPeek(out Item);
                    }
                
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return Item;
            }
            else
            {
                throw new Exception("Queue is empty");
            }
        }
        
        public bool IsEmpty()
        {
            if (this.Size() == 0) return true;
            return false;
        }
    }
}