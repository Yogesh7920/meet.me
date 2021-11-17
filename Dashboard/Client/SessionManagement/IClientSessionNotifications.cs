namespace Dashboard.Client.SessionManagement
{
    /// <summary>
    ///     Interface to notify about changes in the client side
    ///     session data (SessionData Object).
    /// </summary>
    public interface IClientSessionNotifications
    {
        /// <summary>
        ///     Handles the changes in the SessionData object
        /// </summary>
        /// <param name="session"> The changed SessionData </param>
        void OnClientSessionChanged(SessionData session);
    }
}