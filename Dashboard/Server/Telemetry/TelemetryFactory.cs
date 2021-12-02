/// <author>Harsh Parihar</author>
/// <created> 10/11/2021 </created>
/// <summary>
/// Returns an instance of Telemetry
/// </summary>

namespace Dashboard.Server.Telemetry
{
    public static class TelemetryFactory
    {
        private static readonly Telemetry _telemetry;

        static TelemetryFactory()
        {
            if (_telemetry == null) _telemetry = new Telemetry();
        }

        public static Telemetry GetTelemetryInstance()
        {
            return _telemetry;
        }
    }
}