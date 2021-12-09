/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 25 / 11 / 2021
 * Date Modified: 25 / 11 / 2021
* */

using System.Threading;
using Moq;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    internal class ClientCheckPointHandlerTesting
    {
        [SetUp]
        public void SetUp()
        {
            _clientCheckPointHandler = new ClientCheckPointHandler();
            _mockCommunicator = new Mock<IClientBoardCommunicator>();
            _mockCommunicator.Setup(m => m.Subscribe(It.IsAny<IServerUpdateListener>()));

            _clientCheckPointHandler.SetCommunicator(_mockCommunicator.Object);
        }

        private ClientCheckPointHandler _clientCheckPointHandler;
        private Mock<IClientBoardCommunicator> _mockCommunicator;

        [Test]
        public void SaveCheckPoint_RequestSentToCommunicator()
        {
            _mockCommunicator.Setup(m => m.Send(It.IsAny<BoardServerShape>()));
            BoardServerShape expected = new(null, Operation.CreateCheckpoint, "userId", 1);
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

            BoardServerShape expected = new(null, Operation.FetchCheckpoint, "userId", 1);

            _clientCheckPointHandler.FetchCheckpoint(1, "userId", 0);
            _mockCommunicator.Verify(m => m.Send(
                It.Is<BoardServerShape>(obj => StateManagerHelper.CompareBoardServerShapes(obj, expected))
            ), Times.Once());
        }
    }
}