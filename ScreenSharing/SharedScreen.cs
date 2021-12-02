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
        public int messageType;

        // Stores the screen
        public byte[] screen;

        // Stores the user Id of the user sharing the screen
        public string userId;

        // Stores the user name of the user sharing the screen
        public string userName;

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
            userId = uid;
            userName = uname;
            messageType = mtype;
            screen = data;
        }
    }
}