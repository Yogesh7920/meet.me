/// <author>Abdullah Khan</author>
/// <created>14/10/2021</created>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Networking
{
    public class MetaObject
    {
        public string data;
        public string typ;

        public MetaObject()
        {
        }

        public MetaObject(string typ, string data)
        {
            this.data = data;
            this.typ = typ;
        }
    }

    public class Serializer : ISerializer
    {
        /// <inheritdoc />
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                var json = SerializeJSON(objectToSerialize);
                var obj = new MetaObject(typeof(T).ToString(), json);
                return SerializeJSON(obj);
            }
            catch
            {
                var xml = SerializeXML(objectToSerialize);
                var obj = new MetaObject(typeof(T).ToString(), xml);
                return SerializeXML(obj);
                throw;
            }
        }

        /// <inheritdoc />
        string ISerializer.GetObjectType(string serializedString, string nameSpace)
        {
            if (serializedString[0] == '<')
            {
                // xml string
                var obj = deserializeXML<MetaObject>(serializedString);
                return obj.typ;
            }

            if (serializedString[0] == '{')
            {
                // json string
                var obj = deserializeJSON<MetaObject>(serializedString);
                return obj.typ;
            }

            throw new InvalidDataException();
        }

        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                var obj = deserializeJSON<MetaObject>(serializedString);
                return deserializeJSON<T>(obj.data);
            }
            catch (Exception ex1)
            {
                try
                {
                    var obj = deserializeXML<MetaObject>(serializedString);
                    return deserializeXML<T>(obj.data);
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine(ex2.Message);
                    throw;
                }

                Trace.WriteLine(ex1.Message);
                throw;
            }
        }

        private string SerializeJSON<T>(T objectToSerialize)
        {
            var jset = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
            var serializedString = JsonConvert.SerializeObject(objectToSerialize, jset);
            return serializedString;
        }

        private string SerializeXML<T>(T objectToSerialize)
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

        private T deserializeXML<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                return (T) serializer.Deserialize(stringReader);
            }
        }

        private T deserializeJSON<T>(string json)
        {
            var jset = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
            return JsonConvert.DeserializeObject<T>(json, jset);
        }
    }
}