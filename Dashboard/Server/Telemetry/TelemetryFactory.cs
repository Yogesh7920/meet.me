using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Telemetry
{
    public static class TelemetryFactory
    {
        static TelemetryFactory()
        {
            if(_telemetry == null)
            {
                _telemetry = new Telemetry();
            }
        }

        public static Telemetry GetTelemetryInstance()
        {
            return new Telemetry();
        }
        private static Telemetry _telemetry;
    }
}
