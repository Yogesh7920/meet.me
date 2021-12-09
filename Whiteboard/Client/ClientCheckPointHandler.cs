/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 11/1/2021
 * Date Modified: 11/1/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Whiteboard
{
    /// <summary>
    ///     Checkpoint handling at client side
    /// </summary>
    public sealed class ClientCheckPointHandler : IClientCheckPointHandler
    {
        // Check if running as part of NUnit
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        //no. of checkpoints stored on the server

        // Instances of other class
        private IClientBoardCommunicator _clientBoardCommunicator = ClientBoardCommunicator.Instance;

        /// <summary>
        ///     Gets and sets checkpoint number.
        /// </summary>
        public int CheckpointNumber { get; set; }


        /// <summary>
        ///     Fetches the checkpoint from server.
        /// </summary>
        /// <param name="checkpointNumber"></param>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        public void FetchCheckpoint(int checkpointNumber, string UserId, int currentCheckpointState)
        {
            try
            {
                if (checkpointNumber <= CheckpointNumber)
                {
                    //creating boardServerShape object with FetchCheckpoint object
                    List<BoardShape> boardShape = null;
                    var boardServerShape = new BoardServerShape(boardShape,
                        Operation.FetchCheckpoint,
                        UserId,
                        checkpointNumber,
                        currentCheckpointState);

                    //sending boardServerShape object to _clientBoardCommunicator
                    _clientBoardCommunicator.Send(boardServerShape);
                }
                else
                {
                    throw new ArgumentException("invalid checkpointNumber");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.FetchCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     creates and saves the checkpoint
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="currentCheckpointState"></param>
        public void SaveCheckpoint(string UserId, int currentCheckpointState)
        {
            try
            {
                // increasing the checkpoint number by one
                CheckpointNumber++;

                //creating boardServerShape object with CreateCheckpoint object
                List<BoardShape> boardShape = null;
                var boardServerShape = new BoardServerShape(boardShape,
                    Operation.CreateCheckpoint,
                    UserId,
                    CheckpointNumber,
                    currentCheckpointState);

                //sending boardServerShape object to _clientBoardCommunicator
                _clientBoardCommunicator.Send(boardServerShape);
            }

            catch (Exception e)
            {
                Trace.WriteLine("ClientCheckPointHandler.SaveCheckPoint: An exception occured.");
                Trace.WriteLine(e.Message);
            }
        }


        /// <summary>
        ///     Function to set mock communicator when running in test mode.
        /// </summary>
        /// <param name="clientBoardCommunicator">ClientBoardCommunicator instance</param>
        public void SetCommunicator(IClientBoardCommunicator communicator)
        {
            if (IsRunningFromNUnit) _clientBoardCommunicator = communicator;
        }
    }
}