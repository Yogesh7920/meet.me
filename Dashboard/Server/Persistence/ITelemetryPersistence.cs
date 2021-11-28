/// <author>Parmanand Kumar</author>
/// <created>03/11/2021</created>
/// <summary>
///     It contains the public interface required by Telemetry Module to SAVE/RETRIEVE different Analytics data
///     It exposes the basic functinality of Telemetry Module
/// </summary> 

using Dashboard.Server.Telemetry;

//Persistence
namespace Dashboard.Server.Persistence
{
    public interface ITelemetryPersistence
    {
        /// <summary>
        ///     save the UserCountVsTimeStamp, UserIdVsChatCount, InsincereMember data as png after each session.
        /// </summary>
        /// <param name="sessionAnalyticsData"> takes sessionAnalyticsData from Telemetry. </param>
        public ResponseEntity Save(SessionAnalytics sessionAnalyticsData);

        /// <summary>
        ///     append the ServerData into a file after each session end
        /// </summary>
        /// <param name="AllserverData"> takes ServerData from Telemetry to be saved into text file </param>
        /// <returns>Returns true if saved successfully else returns false</returns>
        public ResponseEntity SaveServerData(ServerDataToSave AllserverData);

        /// <summary>
        ///     retrives the ServerData after end of all of the sessions.
        /// </summary>
        /// <returns>returns List of SeverData</returns>
        public ServerDataToSave RetrieveAllSeverData();
    }
}