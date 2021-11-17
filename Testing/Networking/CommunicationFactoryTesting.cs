using Networking;
using NUnit.Framework;

namespace Testing.Networking
{
    [TestFixture]
    class CommunicationFactoryTesting
    {
        [Test]
        public void singleton()
        {
            ICommunicator comm1 = CommunicationFactory.GetCommunicator();
            ICommunicator comm2 = CommunicationFactory.GetCommunicator();

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}
