/// <author>Abdullah Khan</author>
/// <created>14/10/2021</created>
/// <summary>
/// This file contains the class definitions of the serializer submodule.
/// </summary>

using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Networking
{
    /// <summary>
    ///     Wrapper object to store serilized object's type and serilized string representation.
    /// </summary>
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
        private JsonSerializerSettings _jsonSerializerSettings;

        public Serializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All};
        }
        /// <inheritdoc />
        string ISerializer.Serialize<T>(T objectToSerialize)
        {
            try
            {
                var json = SerializeJson(objectToSerialize);
                var obj = new MetaObject(typeof(T).ToString(), json);
                return SerializeJson(obj);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Networking] Error while serializing: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc />
        string ISerializer.GetObjectType(string serializedString, string nameSpace)
        {
            // json string
            var obj = DeserializeJson<MetaObject>(serializedString);
            return obj.typ;
        }

        /// <inheritdoc />
        T ISerializer.Deserialize<T>(string serializedString)
        {
            try
            {
                var obj = DeserializeJson<MetaObject>(serializedString);
                return DeserializeJson<T>(obj.data);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Networking] Error while deserializing: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        ///     JSON supported serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        private string SerializeJson<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, _jsonSerializerSettings);
        }

        /// <summary>
        ///     JSON supoorted deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }
    }
}