/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 14/10/2021
 * date modified: 14/10/2021
**/

using System;
using System.Collections.Generic;
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
		public Queue<SharedScreen> frameQueue;
		// Stores an instance of the Serializer
		public ISerializer serializer;
		//Stores the moduleId which will be "ScreenSharing"
		public string moduleId;
		//Timer will be used to sense disconnection issues.
		public Timer timer;
		//Stores the user Id of the user currently sharing the screen.
		public int userId;

		public static string identifier;

		/// <summary>
		/// Public Constructor which will initialize most of the attributes.
		/// </summary>
		public ScreenShareServer()
		{
			_communicator = CommunicationFactory.GetCommunicator();
			//_communicator.Subscribe();
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be triggered by the Networking team whenever a screen is sent.
		/// </summary>
		public void OnDataReceived(string data)
		{

			throw new NotImplementedException();
		}

		/// <summary>
        /// This method will implement the logic of sharing the required signal.
        /// </summary>
		public void share()
        {
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method will be invoked when no updates are recieved for a certain amount of time.
		/// </summary>
		public void onTimeout()
        {
			throw new NotImplementedException();
		}
	}
}
