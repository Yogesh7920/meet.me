/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 13 Oct 2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Listens to publications from ICommunicator
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        ///     Receives XML strings from ICommunicator
        /// </summary>
        /// <param name="data"> XML string from communicator </param>
        void OnMessageReceived(string data);
    }
}