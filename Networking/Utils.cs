/// <author>Subhash S</author>
/// <created>27/11/2021</created>

namespace Networking
{
    public static class Utils
    {
        public const string Flag = "[FLAG]";
        public const string Esc = "[ESC]";

        /// <summary>
        ///     This method form string from packet object
        ///     it also adds EOF to indicate that the message
        ///     that has been popped out from the queue is finished
        /// </summary>
        /// <param name="packet">Packet Object.</param>
        /// <returns>String </returns>
        public static string GetMessage(Packet packet)
        {
            var msg = packet.ModuleIdentifier;
            msg += ":";
            msg += packet.SerializedData;
            msg = msg.Replace(Esc, $"{Esc}{Esc}");
            msg = msg.Replace(Flag, $"{Esc}{Flag}");
            msg = $"{Flag}{msg}{Flag}";
            return msg;
        }
    }
}