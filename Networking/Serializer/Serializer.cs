/*
 * Author: Abdullah Khan
 * Created on: 14/10/2021
 * Summary: This file contains the class definitions of the serializer submodule.
 */

using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Networking
{
    /// <summary>
    /// Wrapper object to store serilized object's type and serilized string representation.
    /// </summary>
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
        /// <summary>
        /// JSON supported serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
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
        /// <summary>
        /// JSON supoorted deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
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