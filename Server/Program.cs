/**
 * Owned By: Pulavarti Vinay Kumar
 * Created By: Pulavarti Vinay Kumar
 * Date Created: 1 Nov 2021
 * Date Modified: 5 Nov 2021
**/

using System;
using Dashboard.Server.SessionManagement;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ServerSM = new ServerSessionManager();

            // if the input command entered is startMeet, will call the GetPortsAndIPAddress function which creates the meeting and 
            // returns the object which contains IPAddress and Port of the meeting
            var Meeting = ServerSM.GetPortsAndIPAddress();
            Console.WriteLine(Meeting.ipAddress + " : " + Meeting.port);
            Console.WriteLine("Meeting has started by Host");
            while (true)
            {
                // code for listener event to end the meeting
            }
        }
    }
}