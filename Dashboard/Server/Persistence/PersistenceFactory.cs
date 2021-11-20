//Author: Parmanand Kumar

namespace Dashboard.Server.Persistence
{
    //Persisence Factory
    /// <summary>
    ///     create instances of Summamry or Telemetry Persistence Class respectively
    /// </summary>
    public static class PersistenceFactory
    {

        static PersistenceFactory()
        {
            if(_summaryPersisitence == null)
            {
                _summaryPersisitence = new SummaryPersistence();
            }

            if(_telemetryPersisitence == null)
            {
                _telemetryPersisitence = new TelemetryPersistence();
            }

        }
        /// <summary>Helps to create instance of ISummaryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public static ISummaryPersistence GetSummaryPersistenceInstance()
        {
            return new SummaryPersistence();
        }

        /// <summary>Helps to create instance of ITelemetryPersistence </summary>
        /// <returns> return ITelemetryPersistence Interface which enables to use functions of class inheriting the same </returns>
        public static ITelemetryPersistence GetTelemetryPersistenceInstance()
        {
            return new TelemetryPersistence();
        }

        private static ISummaryPersistence _summaryPersisitence;
        private static ITelemetryPersistence _telemetryPersisitence;

    }

}
