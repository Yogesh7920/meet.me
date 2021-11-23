using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.SessionManagement;
using Content;

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

        public static ITelemetry GetTelemetryInstance()
        {
            return new Telemetry();
        }
        private static ITelemetry _telemetry;
    }
}
