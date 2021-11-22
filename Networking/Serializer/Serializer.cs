using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Networking
{
    public class Serializer : ISerializer
    {
        /// <inheritdoc />
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                var serializer = new XmlSerializer(objectToSerialize.GetType());
                using var stringStream = new StringWriter();
                serializer.Serialize(stringStream, objectToSerialize);
                return stringStream.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        string ISerializer.GetObjectType(string serializedString, string nameSpace)
        {
            var stringReader = new StringReader(serializedString);
            var xmlReader = XmlReader.Create(stringReader);
            if (xmlReader.MoveToContent() != XmlNodeType.Element) throw new FormatException();

            var typ = nameSpace + "." + xmlReader.Name;
            return typ;
        }

        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var stringReader = new StringReader(serializedString);
                return (T)serializer.Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }
    }
}