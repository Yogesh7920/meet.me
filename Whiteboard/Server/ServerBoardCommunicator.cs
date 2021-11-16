/**
 * Owned By: Gurunadh Pachappagari
 * Created By: Gurunadh Pachappagari
 * Date Created: 13 Oct 2021
 * Date Modified: 01 Nov 2021
**/

using System;
using Networking;

namespace Whiteboard
{
    /// <summary>
    ///     Bridge the gap between Server side White Board Modules and Networking module
    /// </summary>
    public sealed class ServerBoardCommunicator : INotificationHandler, IServerBoardCommunicator
    {
        private static ServerBoardCommunicator instance;
        private static ISerializer serializer;
        private static ICommunicator communicator;
        private static readonly string moduleIdentifier = "Whiteboard";
        private static ServerBoardStateManager stateManager;

        /// <summary>
        ///     private constructor for a singleton
        /// </summary>
        private ServerBoardCommunicator()
        {
        }

        /// <summary>
        ///     instance getter
        /// </summary>
        public static ServerBoardCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerBoardCommunicator();
                    serializer = new Serializer();
                    communicator = new ServerCommunicator();
                    communicator.Subscribe(moduleIdentifier, instance);
                    stateManager = new ServerBoardStateManager();
                }

                return instance;
            }
        }

        public void OnDataReceived(string data)
        {
            var deserializedObject = serializer.Deserialize<BoardServerShape>(data);
            var userId = deserializedObject.RequesterId;
            if (deserializedObject.OperationFlag == Operation.FETCH_STATE)
            {
                var shapes = stateManager.FetchState(userId);
                Send(shapes, userId);
            }
            else if (deserializedObject.OperationFlag == Operation.FETCH_CHECKPOINT)
            {
                var checkPointNumber = deserializedObject.CheckpointNumber;
                var shapes = stateManager.FetchCheckpoint(checkPointNumber, userId);
                Send(shapes);
            }
            else if (deserializedObject.OperationFlag == Operation.CREATE_CHECKPOINT)
            {
                stateManager.SaveCheckpoint(userId);
            }
            else if (deserializedObject.OperationFlag == Operation.CREATE ||
                     deserializedObject.OperationFlag == Operation.DELETE ||
                     deserializedObject.OperationFlag == Operation.MODIFY)
            {
                stateManager.SaveUpdate(deserializedObject);
            }
            else
            {
                Console.WriteLine("Unidentified Operation at ServerBoardCommunicator");
            }
        }

        /// <summary>
        ///     serializes the shape objects and passes it to communicator.send()
        /// </summary>
        /// <param name="clientUpdate"> the object to be passed to client</param>
        public void Send(BoardServerShape clientUpdate, string clientId = "all")
        {
            var xml_obj = serializer.Serialize(clientUpdate);
            if (clientId == "all")
                communicator.Send(xml_obj, moduleIdentifier);
            else
                communicator.Send(xml_obj, moduleIdentifier, clientId);
        }
    }
}