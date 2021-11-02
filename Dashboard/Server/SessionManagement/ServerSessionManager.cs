using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using System.Diagnostics;

namespace Dashboard.Server.SessionManagement
{
    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager
    {
        /// <summary>
        /// Constructor for the ServerSessionManager, calls the 
        /// tracelistener 
        /// </summary>
        public ServerSessionManager()
        {
            Session session = new Session();
            session.TraceListener();
        }
        /// <summary>
        /// Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The listener of the subscriber </param>
        public void Subcribe(ITelemetryNotifications listener, string identifier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if an IPAddress is valid or not.
        /// </summary>
        /// <param name="IPAddress">The input ipaddress</param>
        /// <returns> true: For valid IP Addresses
        /// false: otherwise</returns>
        bool IsValidIPAddress(string IPAddress)
        {
            // Check for null string, whitespaces or absence of colon
            if (String.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false)
            {
                return false;
            }

            // Take the part before colon as the ip address
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            string[] byteValues = IPAddress.Split('.');

            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4)
            {
                return false;
            }

            // We have 4 bytes in a address
            byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        /// <summary>
        /// Returns the credentials required to 
        /// Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object </returns>
        public MeetingCredentials GetPortsAndIPAddress()
        {
            ICommunicator communicator = CommunicationFactory.GetCommunicator(false);
            Trace.WriteLine("Fetching IP Address and port from the networking module");
            string meetAddress = communicator.Start();

            if (IsValidIPAddress(meetAddress) != true)
            {
                Trace.WriteLine("IP Address is not valid, return null");
                return null;
            }

            Trace.WriteLine("Returning the IP Address to the UX");
            string ipAddress = meetAddress.Substring(0, meetAddress.IndexOf(':'));
            int port = Convert.ToInt16(meetAddress.Substring(meetAddress.IndexOf(':') + 2));

            return new MeetingCredentials(ipAddress, port);
        }
    }
}

