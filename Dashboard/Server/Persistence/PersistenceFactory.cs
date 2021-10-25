using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Server.Persistence
{
    public class PersistenceFactory
    {
            ISummaryPersistence getSummaryPersistenceInstance()
            {
                return new SummaryPersistence();
            }

            ITelemetryPersistence getTelemetryPersistenceInstance()
            {
                return new TelemetryPersistence();
            }

    }

}
