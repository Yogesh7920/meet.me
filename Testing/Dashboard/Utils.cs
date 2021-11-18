using System;
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
            return
                $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}:{random.Next(0, 65535)}";
        }

        public static MeetingCredentials generateMeetingCreds(string ipAddressAndPort)
        {
            var colonIndex = ipAddressAndPort.IndexOf(':');
            if (colonIndex == -1)
                return null;
            var ipAddress = ipAddressAndPort.Substring(0, colonIndex);
            var port = int.Parse(ipAddressAndPort.Substring(colonIndex + 1));
            var _meetCreds = new MeetingCredentials(ipAddress, port);
            return _meetCreds;
        }
    }
}