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
            ServerSessionManager serverSM = SessionManagerFactory.GetServerSessionManager();

            // if the input command entered is startMeet, will call the GetPortsAndIPAddress function which creates the meeting and 
            // returns the object which contains IPAddress and Port of the meeting
            MeetingCredentials meeting = serverSM.GetPortsAndIPAddress();
            Console.WriteLine(meeting.ipAddress + " : " + meeting.port);
            Console.WriteLine("Meeting has started by Host");
        }
    }
}