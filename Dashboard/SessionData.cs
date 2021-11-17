using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard
{
    /// <summary>
    /// This class is used to store the data about the 
    /// current session 
    /// </summary>
    public class SessionData : IEquatable<SessionData>
    {
        /// <summary>
        /// Constructor to initialise and empty list of users
        /// </summary>
        public SessionData()
        {
            if (users == null)
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
        /// Helps to compare two session objects by overriding the 
        /// Equals function of the IEquatable class.
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj as SessionData);
        }

        /// <summary>
        /// Function for comparing two SessionData Objects 
        /// </summary>
        /// <param name="other"> A bool to signify whether the two objects were equal or not </param>
        /// <returns></returns>
        public bool Equals(SessionData other)
        {
            return ScrambledEquals<UserData>(users, other.users);
        }

        /// <summary>
        /// Compare the equality of two lists of objects ignoring the 
        /// order of objects in both of the lists
        /// </summary>
        /// <typeparam name="T"> The type of object the list is made up of </typeparam>
        /// <param name="list1"> The first list of objects </param>
        /// <param name="list2"> The second list of objects </param>
        /// <returns></returns>
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // the List of users in the meeting 
        public int SessionId;
        public List<UserData> users;
    }
}
