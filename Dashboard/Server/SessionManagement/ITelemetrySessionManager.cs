/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the interface for Telemetry to access serveer session manager's method for subscribing.
/// </summary>


namespace Dashboard.Server.SessionManagement
{
    public interface ITelemetrySessionManager
    {
        /// <summary>
        ///     Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        void Subscribe(ITelemetryNotifications listener);
    }
}