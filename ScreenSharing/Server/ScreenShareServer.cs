/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 25/11/2021
**/

using System;
using System.Timers;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Networking;
using System.Diagnostics;

namespace ScreenSharing
{
	/// <summary>
	/// Server Side screen sharing class
	/// </summary>
	public class ScreenShareServer : INotificationHandler
	{
		//Store the Communicator used to send screen over the network.
		public ICommunicator Communicator;

		//Thread to share the required signal to the required machines.
		public Thread SharingThread;

		// Queue to store the incoming frames
		public Queue<SharedScreen> FrameQueue;

		// Stores an instance of the Serializer
		public ISerializer Serializer;

		public bool IsSharing;

		//Timer will be used to sense disconnection issues.
		public System.Timers.Timer Timer;

		//Stores the user Id of the user currently sharing the screen.
		public string UserId;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareServer()
		{
			UserId = "-";
			Timer = new System.Timers.Timer(10000);
			Timer.Elapsed += OnTimeout;
			Timer.AutoReset = true;
            FrameQueue = new Queue<SharedScreen>();

            Communicator = CommunicationFactory.GetCommunicator(false);
			Communicator.Subscribe("ScreenSharing", this);
			Serializer = new Serializer();

			IsSharing = true;
			SharingThread = new Thread(Share);
			SharingThread.Start();
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
			try
			{
				SharedScreen scrn = Serializer.Deserialize<SharedScreen>(data);
				FrameQueue.Enqueue(scrn);
				Trace.WriteLine("[ScreenSharingServer] Data received from Networking");
			}
			catch (Exception e)
			{
				Trace.WriteLine("ScreenSharing: Unable to recieve data");
				Trace.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// This method will implement the logic of sharing the required signal.
		/// </summary>
		public void Share()
		{
			try
            {
				while (IsSharing)
				{
					while (FrameQueue.Count == 0) ;
					Timer.Interval = 10000;
					if (Timer.Enabled == false)
						Timer.Start();
					SharedScreen currScreen = FrameQueue.Dequeue();
					if (UserId == "-")
					{
						// this is the case when server is idle and someone wants to share screen
						UserId = currScreen.UserId;
						if (currScreen.MessageType == 0)
						{
							UserId="-";
							Timer.Stop();
							Timer.Interval = 10000;
						}
						// Broadcasting the screen
						string data = Serializer.Serialize<SharedScreen>(currScreen);
						Communicator.Send(data, "ScreenSharing");
						Trace.WriteLine("[ScreenSharingServer] Data sent to Networking");
					}
					else if (currScreen.UserId != UserId)
					{
						// this is a case of simultaneous sharing and the user who is trying to share has to be rejected
						continue;
					}
					else
					{
						if (currScreen.MessageType == 0)
						{
							UserId="-";
							Timer.Stop();
							Timer.Interval = 10000;
						}
						// Broadcasting the screen
						string data = Serializer.Serialize<SharedScreen>(currScreen);
						Communicator.Send(data, "ScreenSharing");
						Trace.WriteLine("[ScreenSharingServer] Data sent to Networking");
					}
				}
			}
			catch(Exception e)
            {
				Trace.WriteLine("ScreenSharing: Problem in sharing thread");
                Trace.WriteLine(e.Message);
            }
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void OnTimeout(Object source, ElapsedEventArgs e)
		{
            try
			{
				UserId = "-";
				FrameQueue.Clear();
			}
			catch(Exception ex)
            {
				Trace.WriteLine("ScreenSharing:Timeout event not working properly");
                Trace.WriteLine(ex.Message);
            }
		}
		~ScreenShareServer()
		{
			IsSharing = false;
			Timer.Dispose();
		}
	}
}
