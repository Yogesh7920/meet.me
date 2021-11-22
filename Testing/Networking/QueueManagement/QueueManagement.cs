/*
 * Author: Alisetti Sai Vamsi
 * Created on: 01/11/2021
 * Summary: This file contains the unit tests
 *          for the Queue Module.
 */

using Networking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing.Networking.QueueManagement
{
    [TestFixture]
    public class QueueManagement
    {
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

            // Creating packets with random moduleIdentifiers
            var random = new Random();
            for (var i = 0; i < _testPackets.Capacity; i++)
            {
                var moduleIndex = random.Next(0, 4);
                var tempModuleId = _moduleIdentifiers[moduleIndex];
                var item = new Packet
                { ModuleIdentifier = tempModuleId, SerializedData = Message };
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

        private IQueue _queue;
        private List<Packet> _testPackets;
        private List<string> _moduleIdentifiers;

        private static string Message => NetworkingGlobals.GetRandomString();

        [Test]
        public void Enqueue_SinglePacket_SizeShouldBeOne()
        {
            const string moduleId = Modules.ScreenShare;
            var packet = new Packet
            { ModuleIdentifier = moduleId, SerializedData = Message };
            _queue.Enqueue(packet);
            var size = _queue.Size();
            Assert.AreEqual(1, size);
        }

        [Test]
        public void Enqueue_InvalidModuleIdentifier_ThrowsException()
        {
            const string moduleId = Modules.Invalid;
            var packet = new Packet { ModuleIdentifier = moduleId, SerializedData = Message };

            var ex = Assert.Throws<Exception>(() => { _queue.Enqueue(packet); });

            Assert.IsNotNull(ex);
            const string expectedMessage = "Key Error: Packet holds invalid module identifier";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Enqueue_MultiplePackets_SizeShouldMatch()
        {
            // Running enqueue from different threads
            var thread1 = Task.Run(() =>
            {
                for (var i = 0; i < _testPackets.Capacity / 2; i++)
                {
                    var packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });

            var thread2 = Task.Run(() =>
            {
                for (var i = _testPackets.Capacity / 2; i < _testPackets.Capacity; i++)
                {
                    var packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });

            Task.WaitAll(thread1, thread2);
            var size = _queue.Size();
            var expectedSize = _testPackets.Count;
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        public void Clear_FlushingAllPackets_ShouldBeEmpty()
        {
            for (var i = 0; i < _testPackets.Capacity; i++)
            {
                var packet = _testPackets[i];
                _queue.Enqueue(packet);
            }

            _queue.Clear();
            var empty = _queue.IsEmpty();
            Assert.AreEqual(true, empty);
        }

        [Test]
        public void Clear_CalledByMultipleThreadsAtSameTime_ThrowsEmptyQueueException()
        {
            const string moduleId = Modules.ScreenShare;

            var packet = new Packet { ModuleIdentifier = moduleId, SerializedData = Message };
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
                var innerEx = ex.InnerExceptions;
                var clearEx = innerEx.ElementAt(0);
                const string expectedMessage = "Empty Queue cannot be dequeued";
                var empty = _queue.IsEmpty();
                Assert.AreEqual(expectedMessage, clearEx.Message);
                Assert.AreEqual(true, empty);
            }
        }

        [Test]
        public void RegisterModule_DifferentModulesPassingSameIdentifier_ThrowsException()
        {
            const string moduleId = Modules.ScreenShare;
            var priority = Priorities.ScreenShare;
            var ex = Assert.Throws<AggregateException>(() =>
            {
                var thread1 = Task.Run(() => { _queue.RegisterModule(moduleId, priority); });

                var thread2 = Task.Run(() => { _queue.RegisterModule(moduleId, priority); });

                Task.WaitAll(thread1, thread2);
            });
            Assert.IsNotNull(ex);
            var innerEx = ex.InnerExceptions;
            var registerEx = innerEx.ElementAt(0);
            const string expectedMessage = "Adding Queue to MultiLevelQueue Failed!";
            Assert.AreEqual(expectedMessage, registerEx.Message);
        }

        [Test]
        public void RegisterModule_IncorrectPriority_ThrowsException()
        {
            const string moduleId = Modules.ScreenShare;
            const int priority = Priorities.Invalid;
            var ex = Assert.Throws<Exception>(() => { _queue.RegisterModule(moduleId, priority); });
            Assert.IsNotNull(ex);
            const string expectedMessage = "Priority should be positive integer";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Dequeue_QueueIsEmpty_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => { _queue.Dequeue(); });
            Assert.IsNotNull(ex);
            const string expectedMessage = "Cannot Dequeue empty queue";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Peek_QueueIsEmpty_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() => { _queue.Peek(); });
            Assert.IsNotNull(ex);
            const string expectedMessage = "Cannot Peek into empty queue";
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void Dequeue_SinglePacket_QueueIsEmpty()
        {
            const string moduleId = Modules.ScreenShare;
            var data = Message;
            var packet = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            var thread1 = Task.Run(() => { _queue.Enqueue(packet); });
            Task.WaitAll(thread1);
            var thread2 = Task.Run(() =>
            {
                var pkt = _queue.Dequeue();
                Assert.AreEqual(pkt.ModuleIdentifier, moduleId);
                Assert.AreEqual(pkt.SerializedData, data);
            });
            Task.WaitAll(thread2);
            var size = _queue.Size();
            Assert.AreEqual(0, size);
        }

        [Test]
        public void Dequeue_MultiplePackets_SizeIsZero()
        {
            var thread1 = Task.Run(() =>
            {
                for (var i = 0; i < _testPackets.Capacity; i++)
                {
                    var packet = _testPackets[i];
                    _queue.Enqueue(packet);
                }
            });
            Task.WaitAll(thread1);
            var thread2 = Task.Run(() =>
            {
                for (var i = 0; i < _testPackets.Capacity; i++) _queue.Dequeue();
            });
            Task.WaitAll(thread1, thread2);
            var size = _queue.Size();
            Assert.AreEqual(0, size);
        }

        [Test]
        public void Peek_SinglePacket_ShouldPeekSamePacket()
        {
            const string moduleId = Modules.ScreenShare;
            var data = Message;
            var packet = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            _queue.Enqueue(packet);
            var p = _queue.Peek();
            Assert.AreEqual(p.ModuleIdentifier, moduleId);
            Assert.AreEqual(p.SerializedData, data);
        }

        [Test]
        public void Dequeue_CheckingOrder_ReturnsProperOrder()
        {
            const string moduleId1 = Modules.Chat;
            var xData1 = Message;
            var xData2 = Message;
            var xData3 = Message;
            const string moduleId2 = Modules.File;
            var yData1 = Message;
            var yData2 = Message;

            var xPacket1 = new Packet { ModuleIdentifier = moduleId1, SerializedData = xData1 };
            var xPacket2 = new Packet { ModuleIdentifier = moduleId1, SerializedData = xData2 };
            var xPacket3 = new Packet { ModuleIdentifier = moduleId1, SerializedData = xData3 };

            var yPacket1 = new Packet { ModuleIdentifier = moduleId2, SerializedData = yData1 };
            var yPacket2 = new Packet { ModuleIdentifier = moduleId2, SerializedData = yData2 };


            var thread1 = Task.Run(() =>
            {
                _queue.Enqueue(xPacket1);
                _queue.Enqueue(xPacket2);
                _queue.Enqueue(xPacket3);
            });

            var thread2 = Task.Run(() =>
            {
                _queue.Enqueue(yPacket1);
                _queue.Enqueue(yPacket2);
            });

            Task.WaitAll(thread1, thread2);
            var thread3 = Task.Run(() =>
            {
                var p1 = _queue.Dequeue();
                var p2 = _queue.Dequeue();
                var p3 = _queue.Dequeue();
                var p4 = _queue.Dequeue();
                var p5 = _queue.Dequeue();

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
            var data = Message;

            const string moduleId2 = Modules.Chat;
            var data2 = Message;

            const string newModuleId = Modules.Networking;
            var newData = Message;
            const int newPriority = Priorities.Networking;

            var packet1 = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            var packet2 = new Packet { ModuleIdentifier = moduleId2, SerializedData = data2 };
            var packet3 = new Packet { ModuleIdentifier = newModuleId, SerializedData = newData };

            var thread1 = Task.Run(() => { _queue.Enqueue(packet1); });
            var thread2 = Task.Run(() => { _queue.Enqueue(packet2); });
            Task.WaitAll(thread1, thread2);


            var p1 = _queue.Dequeue();
            Assert.AreEqual(moduleId, p1.ModuleIdentifier);
            Assert.AreEqual(data, p1.SerializedData);

            _queue.RegisterModule(newModuleId, newPriority);
            _queue.Enqueue(packet3);

            var p2 = _queue.Dequeue();
            Assert.AreEqual(moduleId2, p2.ModuleIdentifier);
            Assert.AreEqual(data2, p2.SerializedData);

            var p3 = _queue.Dequeue();
            Assert.AreEqual(newModuleId, p3.ModuleIdentifier);
            Assert.AreEqual(newData, p3.SerializedData);
        }
    }
}