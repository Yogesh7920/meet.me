namespace Networking
{
    public class Packet
    {
        
        public string ModuleIdentifier;
        public string SerializedData;
        
        /// <summary>
        /// Packet constructor initializes packet fields.
        /// </summary>
        /// <param name="moduleIdentifier">Unique Id for module.</param>
        /// <param name="serializedData">Serialized data corresponding to the moduleIdentifier.</param>
        public Packet(string moduleIdentifier, string serializedData)
        {
            ModuleIdentifier = moduleIdentifier;
            SerializedData = serializedData;
        }
    }
}