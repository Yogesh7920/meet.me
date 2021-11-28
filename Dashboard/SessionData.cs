/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the SessionData class used to store the list of users in the session.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    /// <summary>
    /// This class is used to store the data about the 
    /// current session 
    /// </summary>
    public class SessionData 
    {
        /// <summary>
        /// Constructor to initialise and empty list of users
        /// </summary>
        public SessionData()
        {
            if(users == null)
            {
                users = new List<UserData>();
            }
        }

        /// <summary>
        /// Adds a user to the list of users in the session
        /// </summary>
        /// <param name="user"> An instance of the UserData class </param>
        public void AddUser(UserData user)
        {
            users.Add(user);
        }

        /// <summary>
        /// Overrides the ToString() method to pring the sessionData object for testing, debugging and logging.
        /// </summary>
        /// <returns> Returns a string which contains the data of each user separated by a newline character </returns>
        public override string ToString()
        {
            string output = "";
            for(int i = 0; i < users.Count; ++i)
            {
                output += users[i].ToString();
                output += "\n";
            }
            return output;
        }

        // the List of users in the meeting 
        public List<UserData> users;
    }
}
