using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Server.SessionManagement;
using Content;

namespace Dashboard.Server.Telemetry
{
    public class TelemetryFactory
    {
        ITelemetry GetTelemetryInstance()
        {
            return new Telemetry();
        }
    }
}
