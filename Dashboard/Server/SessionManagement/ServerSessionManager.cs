using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace Dashboard.Server.SessionManagement
{ 
    public class ServerSessionManager : ITelemetrySessionManager, IUXServerSessionManager
    {
        /// <summary>
        /// Subscribes to changes in the session object
        /// </summary>
        /// <param name="listener"> The subscriber. </param>
        /// <param name="identifier"> The listener of the subscriber </param>
        public void Subcribe(ITelemetryNotifications listener, string identifier)
        {
            throw new NotImplementedException();
        }

        bool IsValidIPAddress(string IPAddress)
        {
            if (String.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false)
            { 
                return false;
            }
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            string[] byteValues = IPAddress.Split('.');

            if(byteValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;
            return byteValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        /// <summary>
        /// Returns the credentials required to 
        /// Join or start the meeting
        /// </summary>
        /// <returns> A MeetingCredentials Object </returns>
        public MeetingCredentials GetPortsAndIPAddress()
        {
            ICommunicator communicator = CommunicationFactory.GetCommunicator();
            string meetAddress = communicator.Start();

            if(IsValidIPAddress(meetAddress) != true)
            {
                return null;
            }

            string ipAddress = meetAddress.Substring(0, meetAddress.IndexOf(':'));
            int port = Convert.ToInt16(meetAddress.Substring(meetAddress.IndexOf(':') + 2));

            return new MeetingCredentials(ipAddress,port);
        }
    }
}
