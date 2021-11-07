/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 7/11/2021
**/

namespace ScreenSharing
{

    public class SharedScreen
    {
        public SharedScreen(string uid,string uname,int mtype,byte[] data)
        {
            userId = uid;
            username = uname;
            messageType = mtype;
            screen = data;
        }

        // Stores the user Id of the user sharing the screen
        public string userId;

        // Stores the user name of the user sharing the screen
        public string username;

        // Stores the type of message.
        public int messageType;

        // Stores the screen
        public byte[] screen;
    }
}
