/// <author>Harsh Parihar</author>
/// <created> 10/11/2021 </created>
/// <summary>
/// Returns an instance of Telemetry
/// </summary>

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
            return _telemetry;
        }
        private static Telemetry _telemetry;
    }
}
