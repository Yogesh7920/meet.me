using System;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                //To read a command from the Terminal
                string s = Console.ReadLine();
                //if the input command entered is startMeet, will call the GetPortsAndIPAddress function which creates the meeting and 
                //returns the object which contains IPAddress and Port of the meeting
                if(s == "startMeet")
                {
                    MeetingCredentials m = GetPortsAndIPAddress();
                    console.writeline(m.ipaddress + ":" + m.port);
                    Console.WriteLine("Meeting has started by Host");
                }
                //if the input command entered is endMeet, will call the EndMeet function which ends the meeting
                else if (s == "endMeet")
                {
                    EndMeet();
                    Console.WriteLine("Meeting has ended by Host");
                }
                else
                {
                    Console.WriteLine("Invalid Command. Try startMeet or endMeet commands!!!");
                }

            }
        }
    }
}