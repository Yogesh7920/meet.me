/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 15/11/2021
**/

using System;
using System.Timers;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Networking;

namespace ScreenSharing
{
	/// <summary>
	/// Server Side screen sharing class
	/// </summary>
	public class ScreenShareServer : INotificationHandler
	{
		//Store the Communicator used to send screen over the network.
		public ICommunicator _communicator;

		//Thread to share the required signal to the required machines.
		public Thread _sharingThread;

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
			Timer = new System.Timers.Timer(2000);
			Timer.Elapsed += OnTimeout;
			Timer.AutoReset = true;

			_communicator = CommunicationFactory.GetCommunicator();
			_communicator.Subscribe(this.GetType().Namespace, this);
			Serializer = new Serializer();

			IsSharing = true;
			_sharingThread = new Thread(Share);
			_sharingThread.Start();
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{
			SharedScreen scrn = Serializer.Deserialize<SharedScreen>(data);
			FrameQueue.Enqueue(scrn);
		}

		/// <summary>
		/// This method will implement the logic of sharing the required signal.
		/// </summary>
		public void Share()
		{
			while (IsSharing)
			{
				while (FrameQueue.Count == 0) ;
				Timer.Interval = 2000;
				if (Timer.Enabled == false)
					Timer.Start();
				SharedScreen currScreen = FrameQueue.Dequeue();
				if (UserId == "-")
				{
					// this is the case when server is idle and someone wants to share screen
					UserId = currScreen.UserId;
				}
				else if (currScreen.UserId != UserId)
				{
					// this is a case of simultaneous sharing and the user who is sharing has to be rejected
					continue;
				}
				else
				{
					if (currScreen.MessageType == 0)
					{
						Timer.Stop();
						Timer.Interval = 2000;
					}
					// Broadcasting the screen
					string data = Serializer.Serialize<SharedScreen>(currScreen);
					_communicator.Send(data, MethodInfo.GetCurrentMethod().ReflectedType.Namespace);
				}
			}
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void OnTimeout(Object source, ElapsedEventArgs e)
		{
			UserId = "-";
			FrameQueue.Clear();
		}
		~ScreenShareServer()
		{
			IsSharing = false;
			Timer.Dispose();
		}
	}
}
