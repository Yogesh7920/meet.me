namespace Dashboard
{
    /// <summary>
    ///     Class for sending data to the client side
    ///     from the server side
    /// </summary>
    public class ServerToClientData
    {
        private readonly IRecievedFromServer _receivedObject;
        private readonly UserData _user;

        public string eventType;

        /// <summary>
        ///     Parametric constructor to initialize the fields
        /// </summary>
        /// <param name="eventName"> The name of the event </param>
        /// <param name="objectToSend"> The object that is to be sent on the client side </param>
        public ServerToClientData(string eventName, IRecievedFromServer objectToSend, UserData user)
        {
            eventType = eventName;
            _receivedObject = objectToSend;
            _user = user;
        }

        /// <summary>
        ///     Default constructor for serialization
        /// </summary>
        public ServerToClientData()
        {
        }

        /// <summary>
        ///     Returns the object sent from the server to the client
        /// </summary>
        /// <returns>An object of type IReceivedFromServer</returns>
        public IRecievedFromServer GetObject()
        {
            return _receivedObject;
        }

        /// <summary>
        ///     Method to access the UserData object
        /// </summary>
        /// <returns> A UserData object containing the details of a user. </returns>
        public UserData GetUser()
        {
            return _user;
        }
    }
}