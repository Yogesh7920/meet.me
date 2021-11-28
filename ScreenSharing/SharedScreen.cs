/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 24/11/2021
**/

namespace ScreenSharing
{
    public class SharedScreen
    {
        // Stores the type of message.
        public int MessageType;

        // Stores the screen
        public byte[] Screen;

        // Stores the user Id of the user sharing the screen
        public string UserId;

        // Stores the user name of the user sharing the screen
        public string Username;

        /// <summary>
        ///     ///
        ///     <summary>
        ///         unparametrized constructor necessary for serialization.
        ///     </summary>
        public SharedScreen()
        {
        }

        /// <summary>
        ///     parametrized constructor.
        /// </summary>
        public SharedScreen(string uid, string uname, int mtype, byte[] data)
        {
            UserId = uid;
            Username = uname;
            MessageType = mtype;
            Screen = data;
        }
    }
}