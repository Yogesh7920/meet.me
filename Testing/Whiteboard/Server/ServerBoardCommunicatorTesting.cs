using Whiteboard;
using NUnit.Framework;

namespace Testing.Whiteboard
{
    [TestFixture]
    class ServerBoardCommunicatorTesting
    {
        [Test]
        public void Instance_Always_ReturnsSameInstance()
        {
            var comm1 = ServerBoardCommunicator.Instance;
            var comm2 = ServerBoardCommunicator.Instance;

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}