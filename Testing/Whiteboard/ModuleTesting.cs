using System;
using Whiteboard;
using Networking;
using Client;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Threading;

namespace Testing.Whiteboard
{

    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class ModuleTesting
    {
        private WhiteBoardOperationHandler _whiteboardOperationHandler;
        private ClientBoardCommunicator _clientBoardCommunicator;
        private ClientBoardStateManager _clientBoardStateManager;
        private readonly Mock<ActiveBoardOperationsHandler> _mockBoardOperationsState;
        private readonly Mock<ClientBoardCommunicator> _mockClientBoardCommunicator;
        private Mock<ISerializer> _mockSerializer;
        private Mock<ICommunicator> _mockCommunicator;
        private readonly String whiteboardIdentifier = "Whiteboard";

        [SetUp]
        public void SetUp()
        {
            _whiteboardOperationHandler = new WhiteBoardOperationHandler(new(0, 0));
            _clientBoardCommunicator = ClientBoardCommunicator.Instance;
            _clientBoardStateManager = ClientBoardStateManager.Instance;
            _clientBoardStateManager.Start();
            _clientBoardStateManager.SetUser("1");
            _mockCommunicator = new Mock<ICommunicator>();
            _mockSerializer = new Mock<ISerializer>();
            _clientBoardCommunicator.SetCommunicatorAndSerializer(_mockCommunicator.Object, _mockSerializer.Object);
        }

        [Test]
        public void AfterShapeCreation_ReceiveNonNullXML()
        {
            
            _whiteboardOperationHandler.CreateRectangle(new(0, 0), new(100, 100), 1, new(0, 0, 0), null, true);

            _mockCommunicator.Setup(m => m.Send(It.IsAny<String>(), It.IsAny<String>()));
            _mockSerializer.Setup(m => m.Serialize<BoardServerShape>(It.IsAny<BoardServerShape>())).Returns("test-string");
            _mockCommunicator.Verify(m => m.Send(It.Is<String>(str => str == "test-string"), It.Is<String>(str => str == whiteboardIdentifier)), Times.AtLeastOnce);
        }

    }
}
