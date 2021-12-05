/// <author> Rajeev Goyal </author>
/// <created> 24/10/2021 </created>
/// <summary>
/// This file contains the interface to listen to changes in  Client session manager's session data.
/// </summary>

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