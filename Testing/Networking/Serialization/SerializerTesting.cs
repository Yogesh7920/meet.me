using AutoFixture;
using FluentAssertions;
using Networking;
using NUnit.Framework;
using System;
using Testing.Networking.Objects;

namespace Testing.Networking
{
    [TestFixture]
    public class SerializerTesting
    {
        [SetUp]
        public void SetUp()
        {
            _ser = new Serializer();
        }

        private ISerializer _ser;

        [Test]
        public void SerializeDeserializeSimpleObject()
        {
            // Instantiate
            var serObj = new Fixture().Create<SimpleObject>();
            // Serialize
            var xml = _ser.Serialize(serObj);
            // Deserialize
            var desObj = _ser.Deserialize<SimpleObject>(xml);
            // Object Comparison
            desObj.Should().BeEquivalentTo(serObj);
        }

        [Test]
        public void SerializeDeserializeComplexObject()
        {
            // Instantiate
            var serObj = new Fixture().Create<ComplexObject>();
            var xml = _ser.Serialize(serObj);
            var desObj = _ser.Deserialize<ComplexObject>(xml);
            desObj.Should().BeEquivalentTo(serObj);
        }

        [Test]
        public void ObjectType()
        {
            // Serialize
            var serObj = new Fixture().Create<ComplexObject>();
            var xml = _ser.Serialize(serObj);
            // Get object type from xml
            const string nameSpace = "Testing.Networking.Objects";
            var typ = _ser.GetObjectType(xml, nameSpace);
            Assert.That(typeof(ComplexObject).ToString() == typ);
        }

        [Test]
        public void BasicDataTypes()
        {
            // Serialize
            var serObj = new Fixture().Create<int>();
            var xml = _ser.Serialize(serObj);
            // Deserialize
            var desObj = _ser.Deserialize<int>(xml);
            Assert.That(serObj == desObj);
        }

        [Test]
        public void NonSerializableAttributeError()
        {
            var serObj = new Fixture().Create<NonSerializableAttribute>();
            Assert.Throws<InvalidOperationException>(() => _ser.Serialize(serObj));
        }

        [Test]
        public void DeserializationFailed()
        {
            // Serialize
            var serObj = new Fixture().Create<SimpleObject>();
            var xml = _ser.Serialize(serObj);
            // Corrupt xml string
            xml = xml[50..];
            Assert.Throws<InvalidOperationException>(() => _ser.Deserialize<SimpleObject>(xml));
        }
    }
}