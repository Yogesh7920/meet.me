using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;

namespace Testing.Dashboard
{
    public static class Utils
    {
        public static string GenerateValidIPAndPort()
        {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var random = new Random();
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
            return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}:{random.Next(0, 65535)}";
        }

        public static MeetingCredentials generateMeetingCreds(string ipAddressAndPort)
        {
            int colonIndex = ipAddressAndPort.IndexOf(':');
            if(colonIndex == -1)
                return null;
            string ipAddress = ipAddressAndPort.Substring(0, colonIndex);
            int port = int.Parse(ipAddressAndPort.Substring(colonIndex + 1));
            MeetingCredentials _meetCreds = new MeetingCredentials(ipAddress,port);
            return _meetCreds;
        }
    }
}
