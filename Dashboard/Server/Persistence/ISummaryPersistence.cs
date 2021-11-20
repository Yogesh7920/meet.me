/// <author>Parmanand Kumar</author>
/// <created>03/11/2021</created>
/// <summary>
///     It contains the public interface required by Summary Module to save Summary data.
///     It exposes the basic functinality of Telemetry Module
/// </summary> 
/// 
namespace Dashboard.Server.Persistence
{
    //SummaryPersistence Interface
    public interface ISummaryPersistence
    {
        /// <summary>
        /// saves the summary of the session into a summary file
        /// </summary>
        /// <param name="message"> takes message string that need to be saved </param>
        /// <returns> return true if succesfully saved else return false </returns>
        public ResponseEntity SaveSummary(string message);
    }

}
