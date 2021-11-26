/**
 * owned by: Pulavarti Vinay Kumar
 * created by: Pulavarti Vinay Kumar
 * date created: 1/10/2021
 * date modified: 22/10/2021
**/

using System;
using Dashboard;
using Dashboard.Server.SessionManagement;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ServerSM = SessionManagerFactory.GetServerSessionManager();
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