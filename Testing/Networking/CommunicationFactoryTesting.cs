using NUnit.Framework;
using Networking;

namespace Testing.Networking
{
    [TestFixture]
    class CommunicationFactoryTesting
    {
        [Test]
        public void GetCommunicator_MustReturnReferenceToSameObject()
        {
            ICommunicator comm1 = CommunicationFactory.GetCommunicator();
            ICommunicator comm2 = CommunicationFactory.GetCommunicator();

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}
