using System;
using NUnit.Framework;
using Networking;
using AutoFixture;
using FluentAssertions;
using Testing.Networking.Objects;

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
