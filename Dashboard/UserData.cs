using System;

namespace Dashboard
{
    /// <summary>
    /// The UserData objects contains the relevant data about a
    /// user.
    /// </summary>
    public class UserData : IEquatable<UserData>
    {
        public UserData()
        {

        }

        /// <summary>
        /// Parametric constructor to initialize the fields 
        /// </summary>
        /// <param name="clientName"> The name of the user. </param>
        /// <param name="clientID"> The ID of the user. </param>
        public UserData(string clientName, int clientID)
        {
            username = clientName;
            userID = clientID;
        }

        /// <summary>
        /// Overriding the Equals class to compare two objects of the
        /// type UserData.
        /// </summary>
        /// <param name="obj"> The object of interest in our case is UserData </param>
        /// <returns>Bool denoting the status of equivalence </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj as UserData);
        }

        /// <summary>
        /// Compare two UserData objects
        /// </summary>
        /// <param name="other">An object of the type UserData </param>
        /// <returns> True if the objects are equal, false otherwise </returns>
        public bool Equals(UserData other)
        {
            if (other == null)
                return false;

            return this.userID.Equals(other.userID) &&
                (
                    object.ReferenceEquals(this.username, other.username) ||
                    this.username != null &&
                    this.username.Equals(other.username)
                );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "UserName: " + this.username + "\n UserID: " + this.userID + "\n";
        }

        public string username;
        public int userID;
    }
}
