using System;

namespace Dashboard
{
    /// <summary>
    ///     The UserData objects contains the relevant data about a
    ///     user.
    /// </summary>
    public class UserData : IEquatable<UserData>
    {
        public int userID;

        public string username;

        /// <summary>
        ///     Parametric constructor to initialize the fields
        /// </summary>
        /// <param name="clientName"> The name of the user. </param>
        /// <param name="clientID"> The ID of the user. </param>
        public UserData(string clientName, int clientID)
        {
            username = clientName;
            userID = clientID;
        }

        /// <summary>
        ///     Compare two UserData objects
        /// </summary>
        /// <param name="other">An object of the type UserData </param>
        /// <returns> True if the objects are equal, false otherwise </returns>
        public bool Equals(UserData other)
        {
            if (other == null)
                return false;

            return userID.Equals(other.userID) &&
                   (
                       ReferenceEquals(username, other.username) ||
                       username != null &&
                       username.Equals(other.username)
                   );
        }

        /// <summary>
        ///     Overriding the Equals class to compare two objects of the
        ///     type UserData.
        /// </summary>
        /// <param name="obj"> The object of interest in our case is UserData </param>
        /// <returns>Bool denoting the status of equivalence </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj as UserData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}