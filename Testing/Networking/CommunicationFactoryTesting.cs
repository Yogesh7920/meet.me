using System;
using NUnit.Framework;
using Networking;
using NUnit.Framework;

namespace Testing.Networking
{
    [TestFixture]
    internal class CommunicationFactoryTesting
    {
        [Test]
        public void GetCommunicator_MustReturnReferenceToSameObject()
        {
            var comm1 = CommunicationFactory.GetCommunicator();
            var comm2 = CommunicationFactory.GetCommunicator();

            Assert.That(ReferenceEquals(comm1, comm2));
        }
    }
}