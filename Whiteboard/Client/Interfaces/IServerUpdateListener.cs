/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 13 Oct 2021
**/

namespace Whiteboard
{
    /// <summary>
    ///     Listens to publications from ClientBoardCommunicator
    /// </summary>
    public interface IServerUpdateListener
    {
        /// <summary>
        ///     Receives deserialized BoardServerShape objects from ClientBoardCommunicator
        /// </summary>
        /// <param name="serverUpdate"> list of deserialized Objects that was sent from ClientBoardCommunicator</param>
        void OnMessageReceived(BoardServerShape serverUpdate);
    }
}