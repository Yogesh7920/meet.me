namespace Networking
{
    public class Packet
    {
        
        public string ModuleIdentifier;
        public string SerializedData;

        public Packet(string moduleIdentifier, string serializedData)
        {
            ModuleIdentifier = moduleIdentifier;
            SerializedData = serializedData;
        }
    }
}