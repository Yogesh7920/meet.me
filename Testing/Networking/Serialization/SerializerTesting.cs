using System;
using NUnit.Framework;
using Networking;
using AutoFixture;
using FluentAssertions;
using Testing.Networking.Objects;

namespace Testing.Networking
{
    [TestFixture]
    public class SerializerTesting
    {
        private ISerializer _ser;
        [SetUp]
        public void SetUp()
        {
            _ser = new Serializer();
            var random = TestContext.CurrentContext.Random;
        }
        [Test]
        public void SerializeDeserializeSimpleObject()
        {
            // Instantiate
            SimpleObject serObj = new Fixture().Create<SimpleObject>();
            // Serialize
            string xml = _ser.Serialize(serObj);
            // Deserialize
            SimpleObject desObj = _ser.Deserialize<SimpleObject>(xml);
            // Object Comparison
            desObj.Should().BeEquivalentTo(serObj);
        }
        [Test]
        public void SerializeDeserializeComplexObject()
        {
            // Instantiate
            ComplexObject serObj = new Fixture().Create<ComplexObject>();
            string xml = _ser.Serialize(serObj);
            ComplexObject desObj = _ser.Deserialize<ComplexObject>(xml);
            desObj.Should().BeEquivalentTo(serObj);
        }
        [Test]
        public void ObjectType()
        {
            // Serialize
            ComplexObject serObj = new Fixture().Create<ComplexObject>();
            string xml = _ser.Serialize(serObj);
            // Get object type from xml
            string nameSpace = "Testing.Networking.Objects";
            string typ = _ser.GetObjectType(xml, nameSpace);
            Assert.That(typeof(ComplexObject).ToString() == typ);
        }
        [Test]
        public void BasicDataTypes()
        {
            // Serialize
            int serObj = new Fixture().Create<int>();
            string xml = _ser.Serialize(serObj);
            // Deserialize
            int desObj = _ser.Deserialize<int>(xml);
            Assert.That(serObj == desObj);
        }
        [Test]
        public void NonSerializableAttributeError()
        {
            NonSerializableAttribute serObj = new Fixture().Create<NonSerializableAttribute>();
            string xml = _ser.Serialize(serObj);
            NonSerializableAttribute des = _ser.Deserialize<NonSerializableAttribute>(xml);
            des.Should().BeEquivalentTo(serObj);
            //Assert.Throws<InvalidOperationException>(() => _ser.Serialize(serObj));
        }
        [Test]
        public void DeserializationFailed()
        {
            // Serialize
            SimpleObject serObj = new Fixture().Create<SimpleObject>();
            string xml = _ser.Serialize(serObj);
            // Corupt xml string
            xml = xml.Substring(50);
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => _ser.Deserialize<SimpleObject>(xml));
        }
    }
}