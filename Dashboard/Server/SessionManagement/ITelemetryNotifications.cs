/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the interface for Telemetry to listen to changes in session data.
/// </summary>


namespace Dashboard.Server.SessionManagement
{
    public interface ITelemetryNotifications
    {
        /// <summary>
        ///     Handles the change in the Global session (SessionData Object)
        /// </summary>
        /// <param name="newSession"> The changed session </param>
        void OnAnalyticsChanged(SessionData newSession);
    }
}