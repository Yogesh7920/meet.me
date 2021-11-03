using System;
using System.Diagnostics;

namespace Server
{
    using Dashboard;
    using Dashboard.Server.SessionManagement;
    class Program
    {
        static void Main(string[] args)
        {
            ServerSessionManager ServerSM = new ServerSessionManager();

            // if the input command entered is startMeet, will call the GetPortsAndIPAddress function which creates the meeting and 
            // returns the object which contains IPAddress and Port of the meeting
            MeetingCredentials Meeting = ServerSM.GetPortsAndIPAddress();
            Console.WriteLine(Meeting.ipAddress + " : " + Meeting.port);
            Console.WriteLine("Meeting has started by Host");
            while (true)
            {
                // code for listener event to end the meeting
            }
        }
    }
}