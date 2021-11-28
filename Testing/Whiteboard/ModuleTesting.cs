/**
 * Owned By: Manas Sharma
 * Created By: Manas Sharma
 * Date Created: 26/11/2021
 * Date Modified: 28/11/2021
**/

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

            _mockCommunicator.Setup(m => m.Send(It.IsAny<String>(), It.IsAny<String>()));
            _mockSerializer.Setup(m => m.Serialize<BoardServerShape>(It.IsAny<BoardServerShape>())).Returns("test-string");
          
            _clientBoardCommunicator.SetCommunicatorAndSerializer(_mockCommunicator.Object, _mockSerializer.Object);
        }

        /// <summary>
        /// Function to test that all the operations are completed before passing the output through global communicator
        /// </summary>
        [Test]
        public void AfterShapeCreation_ReceiveNonNullXML()
        {
            // initialize a shape object
            _whiteboardOperationHandler.CreateRectangle(new(0, 0), new(100, 100), 1, new(0, 0, 0), null, true);

            // setup a mock send function through communicator
            _mockCommunicator.Setup(m => m.Send(It.IsAny<String>(), It.IsAny<String>()));
            _mockSerializer.Setup(m => m.Serialize<BoardServerShape>(It.IsAny<BoardServerShape>())).Returns("test-string");

            // verify that the serialized string matches
            // Also this completes the test for correct flow of output through the communicator
            _mockCommunicator.Verify(m => m.Send(It.Is<String>(str => str == "test-string"), It.Is<String>(str => str == whiteboardIdentifier)), Times.AtLeastOnce);
        }

    }
}
