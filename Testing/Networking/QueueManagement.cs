using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Networking;
using NuGet.Frameworks;

namespace Testing.Networking
{
    [TestFixture]
    public class QueueManagement
    {
        private IQueue _queue;
        private List<Packet> _testPackets;
        private List<string> _moduleIdentifiers;

        private string Message => NetworkingGlobals.GetRandomString();

        [SetUp]
        public void Setup()
        {
            _queue = new Queue();
            _testPackets = new List<Packet>(100);
            _moduleIdentifiers = new List<string>(4);

            const string screenShareModuleId = Modules.ScreenShare;
            const string whiteBoardModuleId = Modules.WhiteBoard;
            const string chatModuleId = Modules.Chat;
            const string fileModuleId = Modules.File;

            _moduleIdentifiers.Add(screenShareModuleId);
            _moduleIdentifiers.Add(whiteBoardModuleId);
            _moduleIdentifiers.Add(chatModuleId);
            _moduleIdentifiers.Add(fileModuleId);

            const int screenSharePriority = Priorities.ScreenShare;
            const int whiteBoardPriority = Priorities.WhiteBoard;
            const int chatPriority = Priorities.Chat;
            const int filePriority = Priorities.File;

            _queue.RegisterModule(screenShareModuleId, screenSharePriority);
            _queue.RegisterModule(whiteBoardModuleId, whiteBoardPriority);
            _queue.RegisterModule(chatModuleId, chatPriority);
            _queue.RegisterModule(fileModuleId, filePriority);

            Random random = new Random();

            // Creating packets with random moduleIdentifiers
            for (int i = 0; i < _testPackets.Capacity; i++)
            {
                var moduleIndex = random.Next(0, 4);
                var tempModuleId = _moduleIdentifiers[moduleIndex];
                Packet item = new Packet
                    {ModuleIdentifier = tempModuleId, SerializedData = Message};
                _testPackets.Add(item);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Dereference objects after the end of each test
            _moduleIdentifiers = null;
            _queue = null;
            _testPackets = null;
        }

        [Test]
        public void Enqueue_SinglePacket_SizeShouldBeOne()
        {
            const string moduleId = Modules.ScreenShare;
            int size = 0;
            Packet packet = new Packet
                {ModuleIdentifier = moduleId, SerializedData = Message};
            _queue.Enqueue(packet);
            size = _queue.Size();
            Assert.AreEqual(1, size);
        }

        [Test]
        public void Enqueue_InvalidModuleIdentifier_ThrowsException()
        {
            const string moduleId = Modules.Invalid;
            Packet packet = new Packet {ModuleIdentifier = moduleId, SerializedData = Message};

            Exception ex = Assert.Throws<Exception>(() => { _queue.Enqueue(packet); });

            Assert.IsNotNull(ex);
            string expectedMessage = "Key Error: Packet holds invalid module identifier";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Enqueue_MultiplePackets_SizeShouldMatch()
        {
            int size = 0;

            // Running enqueue from different threads
            Task thread1 = Task.Run(() =>
            {
                for (int i = 0; i < _testPackets.Capacity / 2; i++)
                {
                    Packet packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });

            Task thread2 = Task.Run(() =>
            {
                for (int i = _testPackets.Capacity / 2; i < _testPackets.Capacity; i++)
                {
                    Packet packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });

            Task.WaitAll(thread1, thread2);
            size = _queue.Size();
            int expectedSize = _testPackets.Count;
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        public void Clear_FlushingAllPackets_ShouldBeEmpty()
        {
            bool empty;

            for (int i = 0; i < _testPackets.Capacity; i++)
            {
                Packet packet = _testPackets[i];
                _queue.Enqueue(packet);
            }

            _queue.Clear();
            empty = _queue.IsEmpty();
            Assert.AreEqual(true, empty);
        }

        [Test]
        public void Clear_CalledByMultipleThreadsAtSameTime_ThrowsEmptyQueueException()
        {
            const string moduleId = Modules.ScreenShare;
            bool empty;

            Packet packet = new Packet {ModuleIdentifier = moduleId, SerializedData = Message};
            _queue.Enqueue(packet);

            try
            {
                Parallel.Invoke(
                    () => { _queue.Clear(); },
                    () => { _queue.Clear(); }
                );
                Assert.Pass();
            }
            catch (AggregateException ex)
            {
                Assert.IsNotNull(ex);
                ReadOnlyCollection<Exception> innerEx = ex.InnerExceptions;
                Exception clearEx = innerEx.ElementAt(0);
                string expectedMessage = "Empty Queue cannot be dequeued";
                empty = _queue.IsEmpty();
                Assert.AreEqual(expectedMessage, clearEx.Message);
                Assert.AreEqual(true, empty);
            }
        }

        [Test]
        public void RegisterModule_DifferentModulesPassingSameIdentifier_ThrowsException()
        {
            string moduleId = Modules.ScreenShare;
            int priority = Priorities.ScreenShare;
            AggregateException ex = Assert.Throws<AggregateException>(() =>
            {
                Task thread1 = Task.Run(() => { _queue.RegisterModule(moduleId, priority); });

                Task thread2 = Task.Run(() => { _queue.RegisterModule(moduleId, priority); });

                Task.WaitAll(thread1, thread2);
            });
            Assert.IsNotNull(ex);
            ReadOnlyCollection<Exception> innerEx = ex.InnerExceptions;
            Exception registerEx = innerEx.ElementAt(0);
            string expectedMessage = "Adding Queue to MultiLevelQueue Failed!";
            Assert.AreEqual(expectedMessage, registerEx.Message);
        }

        [Test]
        public void RegisterModule_IncorrectPriority_ThrowsException()
        {
            string moduleId = Modules.ScreenShare;
            int priority = Priorities.Invalid;
            Exception ex = Assert.Throws<Exception>(() => { _queue.RegisterModule(moduleId, priority); });
            Assert.IsNotNull(ex);
            string expectedMessage = "Priority should be positive integer";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Dequeue_QueueIsEmpty_ThrowsException()
        {
            Exception ex = Assert.Throws<Exception>(() => { _queue.Dequeue(); });
            Assert.IsNotNull(ex);
            string expectedMessage = "Cannot Dequeue empty queue";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Peek_QueueIsEmpty_ThrowsException()
        {
            Exception ex = Assert.Throws<Exception>(() => { _queue.Peek(); });
            Assert.IsNotNull(ex);
            string expectedMessage = "Cannot Peek into empty queue";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Dequeue_SinglePacket_QueueIsEmpty()
        {
            const string moduleId = Modules.ScreenShare;
            string data = Message;
            int size = 8;
            Packet packet = new Packet {ModuleIdentifier = moduleId, SerializedData = data};
            Task thread1 = Task.Run(() => { _queue.Enqueue(packet); });
            Task.WaitAll(thread1);
            Task thread2 = Task.Run(() =>
            {
                Packet pckt = _queue.Dequeue();
                Assert.AreEqual(pckt.ModuleIdentifier, moduleId);
                Assert.AreEqual(pckt.SerializedData, data);
            });
            Task.WaitAll(thread2);
            size = _queue.Size();
            Assert.AreEqual(0, size);
        }

        [Test]
        public void Dequeue_MultiplePackets_SizeIsZero()
        {
            int size = 0;
            Task thread1 = Task.Run(() =>
            {
                for (int i = 0; i < _testPackets.Capacity; i++)
                {
                    Packet packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });
            Task.WaitAll(thread1);
            Task thread2 = Task.Run(() =>
            {
                for (int i = 0; i < _testPackets.Capacity; i++)
                {
                    Packet packet = _queue.Dequeue();
                }
            });
            Task.WaitAll(thread1, thread2);
            size = _queue.Size();
            Assert.AreEqual(0, size);
        }

        [Test]
        public void Peek_SinglePacket_ShouldPeekSamePacket()
        {
            const string moduleId = Modules.ScreenShare;
            string data = Message;
            Packet packet = new Packet {ModuleIdentifier = moduleId, SerializedData = data};
            _queue.Enqueue(packet);
            Packet p = _queue.Peek();
            Assert.AreEqual(p.ModuleIdentifier, moduleId);
            Assert.AreEqual(p.SerializedData, data);
        }

        [Test]
        public void Dequeue_CheckingOrder_ReturnsProperOrder()
        {
            const string moduleId1 = Modules.Chat;
            string xData1 = Message;
            string xData2 = Message;
            string xData3 = Message;
            const string moduleId2 = Modules.File;
            string yData1 = Message;
            string yData2 = Message;

            Packet xPacket1 = new Packet {ModuleIdentifier = moduleId1, SerializedData = xData1};
            Packet xPacket2 = new Packet {ModuleIdentifier = moduleId1, SerializedData = xData2};
            Packet xPacket3 = new Packet {ModuleIdentifier = moduleId1, SerializedData = xData3};

            Packet yPacket1 = new Packet {ModuleIdentifier = moduleId2, SerializedData = yData1};
            Packet yPacket2 = new Packet {ModuleIdentifier = moduleId2, SerializedData = yData2};


            Task thread1 = Task.Run(() =>
            {
                _queue.Enqueue(xPacket1);
                _queue.Enqueue(xPacket2);
                _queue.Enqueue(xPacket3);
            });

            Task thread2 = Task.Run(() =>
            {
                _queue.Enqueue(yPacket1);
                _queue.Enqueue(yPacket2);
            });

            Task.WaitAll(thread1, thread2);
            Task thread3 = Task.Run(() =>
            {
                Packet p1 = _queue.Dequeue();
                Packet p2 = _queue.Dequeue();
                Packet p3 = _queue.Dequeue();
                Packet p4 = _queue.Dequeue();
                Packet p5 = _queue.Dequeue();
                
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(moduleId1, p1.ModuleIdentifier);
                    Assert.AreEqual(xData1, p1.SerializedData);

                    Assert.AreEqual(moduleId1, p2.ModuleIdentifier);
                    Assert.AreEqual(xData2, p2.SerializedData);

                    Assert.AreEqual(moduleId2, p3.ModuleIdentifier);
                    Assert.AreEqual(yData1, p3.SerializedData);

                    Assert.AreEqual(moduleId1, p4.ModuleIdentifier);
                    Assert.AreEqual(xData3, p4.SerializedData);
                    
                    Assert.AreEqual(moduleId2, p5.ModuleIdentifier);
                    Assert.AreEqual(yData2, p5.SerializedData); 
                });
            });

            Task.WaitAll(thread3);
        }

        [Test]
        public void Dequeue_RegisteringModuleAmidst_ShouldNotAlterDequeueFunctionality()
        {
            const string moduleId = Modules.ScreenShare;
            string data = Message;

            const string moduleId2 = Modules.Chat;
            string data2 = Message;

            const string newModuleId = Modules.Networking;
            string newData = Message;
            int newPriority = Priorities.Networking;

            Packet packet1 = new Packet {ModuleIdentifier = moduleId, SerializedData = data};
            Packet packet2 = new Packet {ModuleIdentifier = moduleId2, SerializedData = data2};
            Packet packet3 = new Packet {ModuleIdentifier = newModuleId, SerializedData = newData};

            Task thread1 = Task.Run(() => { _queue.Enqueue(packet1); });
            Task thread2 = Task.Run(() => { _queue.Enqueue(packet2); });
            Task.WaitAll(thread1, thread2);


            Packet p1 = _queue.Dequeue();
            Assert.AreEqual(moduleId, p1.ModuleIdentifier);
            Assert.AreEqual(data, p1.SerializedData);

            _queue.RegisterModule(newModuleId, newPriority);
            _queue.Enqueue(packet3);

            Packet p2 = _queue.Dequeue();
            Assert.AreEqual(moduleId2, p2.ModuleIdentifier);
            Assert.AreEqual(data2, p2.SerializedData);

            Packet p3 = _queue.Dequeue();
            Assert.AreEqual(newModuleId, p3.ModuleIdentifier);
            Assert.AreEqual(newData, p3.SerializedData);
        }
    }
}