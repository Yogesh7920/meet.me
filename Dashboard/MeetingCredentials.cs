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
        /// the meeting
        /// </summary>
        
        public MeetingCredentials(string address, int portNumber)
        {
            ipAddress = address;
            port = portNumber;
        }
        public string ipAddress;
        public int port;
    }
}

