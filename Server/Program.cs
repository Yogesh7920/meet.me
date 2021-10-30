using System;
using System.Diagnostics;

namespace Server
{
    using Dashboard;
    using Dashboard.Client.SessionManagement;
    using Dashboard.Server.SessionManagement;
    class Program
    {
        static void Main(string[] args)
        {
            //IUXClientSM UXClientSM = SMFactory.GetClientSessionManager();
            IUXClientSessionManager UXClientSM = new ClientSessionManager();
            ServerSessionManager ServerSM = new ServerSessionManager();
            while (true)
            {
                // To read a command from the Terminal
                string Command = Console.ReadLine();
                if (Command == "StartMeet")
                {
                    // if the input command entered is startMeet, will call the GetPortsAndIPAddress function which creates the meeting and 
                    // returns the object which contains IPAddress and Port of the meeting
                    MeetingCredentials Meeting = ServerSM.GetPortsAndIPAddress();
                    Console.WriteLine(Meeting.ipAddress + " : " + Meeting.port);
                    Console.WriteLine("Meeting has started by Host");
                }
                else if (Command == "EndMeet")
                {
                    // if the input command entered is endMeet, will call the EndMeet function which ends the meeting
                    UXClientSM.EndMeet();
                    Console.WriteLine("Meeting has ended by Host");
                }
                else
                {
                    Console.WriteLine("Invalid Command. Try StartMeet or EndMeet commands!!!");
                }
            }
        }
    }
}