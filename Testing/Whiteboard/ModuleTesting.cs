/**
 * Owned By: Manas Sharma
 * Created By: Manas Sharma
 * Date Created: 26/11/2021
 * Date Modified: 28/11/2021
**/

using System.Threading;
using Moq;
using Networking;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    internal class ModuleTesting
    {
        [SetUp]
        public void SetUp()
        {
            _whiteboardOperationHandler = new WhiteBoardOperationHandler(new Coordinate(0, 0));
            _clientBoardCommunicator = ClientBoardCommunicator.Instance;
            _clientBoardStateManager = ClientBoardStateManager.Instance;
            _clientBoardStateManager.Start();
            _clientBoardStateManager.SetUser("1");
            _mockCommunicator = new Mock<ICommunicator>();
            _mockSerializer = new Mock<ISerializer>();

            _mockCommunicator.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>()));
            _mockSerializer.Setup(m => m.Serialize(It.IsAny<BoardServerShape>())).Returns("test-string");

            _clientBoardCommunicator.SetCommunicatorAndSerializer(_mockCommunicator.Object, _mockSerializer.Object);
        }

        private WhiteBoardOperationHandler _whiteboardOperationHandler;
        private ClientBoardCommunicator _clientBoardCommunicator;
        private ClientBoardStateManager _clientBoardStateManager;
        private readonly Mock<ActiveBoardOperationsHandler> _mockBoardOperationsState;
        private readonly Mock<ClientBoardCommunicator> _mockClientBoardCommunicator;
        private Mock<ISerializer> _mockSerializer;
        private Mock<ICommunicator> _mockCommunicator;
        private readonly string whiteboardIdentifier = "Whiteboard";

        /// <summary>
        ///     Function to test that all the operations are completed before passing the output through global communicator
        /// </summary>
        [Test]
        public void AfterShapeCreation_ReceiveNonNullXML()
        {
            // initialize a shape object
            _whiteboardOperationHandler.CreateRectangle(new Coordinate(0, 0), new Coordinate(100, 100), 1,
                new BoardColor(0, 0, 0), null, true);

            // setup a mock send function through communicator
            _mockCommunicator.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>()));
            _mockSerializer.Setup(m => m.Serialize(It.IsAny<BoardServerShape>())).Returns("test-string");

            // verify that the serialized string matches
            // Also this completes the test for correct flow of output through the communicator
            _mockCommunicator.Verify(
                m => m.Send(It.Is<string>(str => str == "test-string"),
                    It.Is<string>(str => str == whiteboardIdentifier)), Times.AtLeastOnce);
        }
    }
}