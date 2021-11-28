using Whiteboard;
using NUnit.Framework;

namespace Testing.Whiteboard
{
    [TestFixture]
    class ClientBoardCommunicatorTesting
    {
        [Test]
        public void Instance_Always_ReturnsSameInstance()
        {
            var comm1 = ClientBoardCommunicator.Instance;
            var comm2 = ClientBoardCommunicator.Instance;

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}