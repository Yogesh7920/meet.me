/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the SessionData class used to store the list of users in the session.
/// </summary>

using System.Collections.Generic;

namespace Dashboard
{
    /// <summary>
    ///     This class is used to store the data about the
    ///     current session
    /// </summary>
    public class SessionData
    {
        // the List of users in the meeting 
        public List<UserData> users;

        /// <summary>
        ///     Constructor to initialise and empty list of users
        /// </summary>
        public SessionData()
        {
            if (users == null) users = new List<UserData>();
        }

        /// <summary>
        ///     Adds a user to the list of users in the session
        /// </summary>
        /// <param name="user"> An instance of the UserData class </param>
        public void AddUser(UserData user)
        {
            users.Add(user);
        }

        /// <summary>
        ///     Overrides the ToString() method to pring the sessionData object for testing, debugging and logging.
        /// </summary>
        /// <returns> Returns a string which contains the data of each user separated by a newline character </returns>
        public override string ToString()
        {
            var output = "";
            for (var i = 0; i < users.Count; ++i)
            {
                output += users[i].ToString();
                output += "\n";
            }

            return output;
        }

        /// <summary>
        ///     Removes the user from the user list in the sessionData.
        /// </summary>
        /// <param name="userID">The UserID of the user who is to be removed</param>
        /// <param name="username"> An optional paramter indicating the name of the user. </param>
        /// <returns></returns>
        public UserData RemoveUserFromSession(int userID, string username = null)
        {
            // Check if the user is in the list and if so, then remove it and return true
            for (var i = 0; i < users.Count; ++i)
                if (users[i].userID.Equals(userID))
                    lock (this)
                    {
                        UserData removedUser = new(users[i].username, users[i].userID);
                        users.RemoveAt(i);
                        return removedUser;
                    }

            return null;
        }
    }
}