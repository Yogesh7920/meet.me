//Author: Parmanand Kumar

namespace Dashboard.Server.Persistence
{
    //Persisence Factory
    /// <summary>
    ///     create instances of Summamry or Telemetry Persistence Class respectively
    /// </summary>
    public class PersistenceFactory
    {
        /// <summary>Helps to create instance of ISummaryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public ISummaryPersistence GetSummaryPersistenceInstance()
        {
            return new SummaryPersistence();
        }

        /// <summary>Helps to create instance of ITelemetryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public ITelemetryPersistence GetTelemetryPersistenceInstance()
        {
            return new TelemetryPersistence();
        }

    }

}
