/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 25 / 11 / 2021
 * Date Modified: 25 / 11 / 2021
* */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Whiteboard;
namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class ClientCheckPointHandlerTesting
    {
        private ClientCheckPointHandler _clientCheckPointHandler;
        private Mock<IClientBoardCommunicator> _mockCommunicator;

        
        [SetUp]
        public void SetUp()
        {
            _clientCheckPointHandler = new();
            _mockCommunicator = new();
            _mockCommunicator.Setup(m => m.Subscribe(It.IsAny<IServerUpdateListener>()));

            _clientCheckPointHandler.SetCommunicator(_mockCommunicator.Object);
        }

        [Test]
        public void SaveCheckPoint_RequestSentToCommunicator()
        {
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            BoardServerShape expected = new(null, Operation.CREATE_CHECKPOINT, "userId",1);
            _clientCheckPointHandler.SaveCheckpoint("userId", 0);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
                ), Times.Once());
        }

        [Test]
        public void FetchCheckPoint_RequestSentToCommunicator()
        {
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            _clientCheckPointHandler.SaveCheckpoint("userId", 0);

            BoardServerShape expected = new(null, Operation.FETCH_CHECKPOINT, "userId", 1);

            _clientCheckPointHandler.FetchCheckpoint(1, "userId", 0);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
                ), Times.Once());

        }

        [Test]
        public void FetchCheckPoint_RequestSentToCommunicatorFails()
        {
            _clientCheckPointHandler.SaveCheckpoint("userId", 0);

            var ex=Assert.Throws<ArgumentException>(() => _clientCheckPointHandler.FetchCheckpoint(2,"userId", 0));
            Assert.That(ex.Message, Is.EqualTo("invalid checkpointNumber"));



        }

    }
}
