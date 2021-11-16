using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard;

namespace Testing.Dashboard
{
    /// <summary>
    /// Utility class for testing
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Generates valid IPs and Ports
        /// </summary>
        /// <returns> A valid IP and port {IP:port}</returns>
        public static string GenerateValidIPAndPort()
        {
            return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}:{random.Next(0, 65535)}";
        }

        /// <summary>
        /// Generates meeting credentials given Ip and port
        /// </summary>
        /// <param name="ipAddressAndPort"> IP and port in the form {IP:port}</param>
        /// <returns>returns corresponding meeting credentials</returns>
        public static MeetingCredentials GenerateMeetingCreds(string ipAddressAndPort)
        {
            int colonIndex = ipAddressAndPort.IndexOf(':');
            if(colonIndex == -1)
                return null;
            string ipAddress = ipAddressAndPort.Substring(0, colonIndex);
            int port = int.Parse(ipAddressAndPort.Substring(colonIndex + 1));
            MeetingCredentials _meetCreds = new MeetingCredentials(ipAddress,port);
            return _meetCreds;
        }

        private static string GetRandomString(int length)
        {

            String b = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_.";


            String randomString = "";

            for (int i = 0; i < length; i++)
            {
                int a = random.Next(54);
                randomString += b.ElementAt(a);
            }
            return randomString;
        }

        /// <summary>
        /// Generates random user data
        /// </summary>
        /// <returns>returns list of users for testing</returns>
        public static List<UserData> GenerateUserData()
        {
            List<UserData> users = new List<UserData>();
            for(int i=0;i<10; i++)
            {
                users.Add(new(GetRandomString(random.Next(10)),i));
            }
            return users;
        }

        /// <summary>
        /// Generates invalid IP and ports
        /// </summary>
        /// <returns> List of invalid IP and ports</returns>
        public static List<string> GenerateInvalidIPAndPort()
        {
            List<string> ipAndPorts = new List<string>();
            ipAndPorts.Add("");
            ipAndPorts.Add(null);
            ipAndPorts.Add("abcd.192.1.2:8080");
            ipAndPorts.Add("192.1.2:8080");
            ipAndPorts.Add("abcdefg");
            return ipAndPorts; 
        }
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        private static Random random = new Random();
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
    }
}
