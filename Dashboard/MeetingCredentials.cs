using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class MeetingCredentials
    {
        /// <summary>
        /// Instances of this class will store the 
        /// credentrials required to join/start
        /// </summary>
        /// <param name="address"> String parameter to store the IP Address </param>
        /// <param name="portNumber"> Int parameter for the port number </param>
        public MeetingCredentials(string address, int portNumber)
        {
            ipAddress = address;
            port = portNumber;
        }
        public string ipAddress;
        public int port;
    }
}
