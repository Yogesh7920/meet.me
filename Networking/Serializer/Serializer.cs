using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json;

namespace Networking
{
    public class MetaObject
    {
        public string typ;
        public string data;
        public MetaObject() { }
        public MetaObject(string typ, string data)
        {
            this.data = data;
            this.typ = typ;
        }
    }
    public class Serializer : ISerializer
    {
        string SerializeJSON<T>(T objectToSerialize)
        {
            string serializedString = JsonConvert.SerializeObject(objectToSerialize);
            return serializedString;
        }
        string SerializeXML<T>(T objectToSerialize)
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
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                string xml = SerializeXML(objectToSerialize);
                MetaObject obj = new MetaObject(typeof(T).ToString(), xml);
                return SerializeXML<MetaObject>(obj);
            }
            catch
            {
                try
                {
                    string json = SerializeJSON(objectToSerialize);
                    MetaObject obj = new MetaObject(typeof(T).ToString(), json);
                    return SerializeJSON<MetaObject>(obj);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <inheritdoc />
        string ISerializer.GetObjectType(string serializedString, string nameSpace)
        {
            try
            {
                if (serializedString[0] == '<')
                {
                    // xml string
                    MetaObject obj = deserializeXML<MetaObject>(serializedString);
                    return obj.typ;
                }
                if (serializedString[0] == '{')
                {
                    // json string
                    MetaObject obj = deserializeJSON<MetaObject>(serializedString);
                    return obj.typ;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
            catch
            {
                throw;
            }
        }
        T deserializeXML<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xml))
                return (T)serializer.Deserialize(stringReader);
        }
        T deserializeJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                MetaObject obj = deserializeXML<MetaObject>(serializedString);
                return deserializeXML<T>(obj.data);
            }
            catch (Exception ex)
            {
                try
                {
                    MetaObject obj = deserializeJSON<MetaObject>(serializedString);
                    return deserializeJSON<T>(obj.data);
                }
                catch
                {
                    Trace.WriteLine(ex.Message);
                    throw;
                }
            }
        }
    }
}