/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the UserData class whose objects store the name and ID of the user.
/// </summary>

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
        ///     Default Constructo for serializing
        /// </summary>
        public UserData()
        {
        }

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
        ///     IEquatable interface consists of this function. This servers as the default
        ///     hash function.
        /// </summary>
        /// <returns> A hash code of the current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     Overriding the ToString() method to print the UserData object for debugging and logging
        /// </summary>
        /// <returns> A string containing the name and the ID of the user</returns>
        public override string ToString()
        {
            return "UserName: " + username + "\n UserID: " + userID + "\n";
        }
    }
}