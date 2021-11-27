using System;
using System.Diagnostics;
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
            var jset = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.SerializeObject(objectToSerialize, Newtonsoft.Json.Formatting.Indented, jset);
        }
        /// <inheritdoc />
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                string json = SerializeJSON(objectToSerialize);
                MetaObject obj = new MetaObject(typeof(T).ToString(), json);
                return SerializeJSON<MetaObject>(obj);
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
            try
            {
                // json string
                MetaObject obj = deserializeJSON<MetaObject>(serializedString);
                return obj.typ;
            }
            catch
            {
                throw;
            }
        }
        T deserializeJSON<T>(string json)
        {
            var jset = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<T>(json, jset);
        }
        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                MetaObject obj = deserializeJSON<MetaObject>(serializedString);
                return deserializeJSON<T>(obj.data);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }
    }
}