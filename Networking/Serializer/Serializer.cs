using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Networking
{
    public class Serializer : ISerializer
    {
        /// <inheritdoc />
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());
            using (var stringStream = new StringWriter())
            {
                serializer.Serialize(stringStream, objectToSerialize);
                return stringStream.ToString();
            }
        }

        /// <inheritdoc />
        string ISerializer.GetObjectType(string serializedString, string nameSpace)
        {
            var stringReader = new StringReader(serializedString);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            if (xmlReader.MoveToContent() != XmlNodeType.Element)
            {
                throw new FormatException();
            }

            string typ = nameSpace + "." + xmlReader.Name;
            return typ;
        }

        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(serializedString))
            {
                return (T) serializer.Deserialize(stringReader);
            }
        }
    }
}