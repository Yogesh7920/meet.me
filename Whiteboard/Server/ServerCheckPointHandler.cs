/**
 * Owned By: Chandan Srivastava
 * Created By: Chandan Srivastava
 * Date Created: 11/1/2021
 * Date Modified: 11/1/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Whiteboard
{
    public sealed class ServerCheckPointHandler : IServerCheckPointHandler
    {
        //List of checkpoints to keep track of checkpoint number and corressponding userId and boardShape
        private readonly List<Tuple<int, string, List<BoardShape>>> _checkpointSummary = new();
        private int _checkpointNumber;

        /// <summary>
        ///     Fetches the checkpoint corresponding to the checkPointNumber
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <returns>Returns list of BoardShape summarzing the checkpoint to the ServerBoardStateManager.</returns>
        public List<BoardShape> FetchCheckpoint(int checkpointNumber)
        {
            try
            {
                if (checkpointNumber > _checkpointNumber) throw new ArgumentException("invalid checkpointNumber");
                // Construct path from the checkpointNumber
                var boardShapesPath = checkpointNumber + ".json";


                // Get the file corresponding to the path
                var jsonString = File.ReadAllText(boardShapesPath);

                //Deserializing the file
                var boardShapes = JsonConvert.DeserializeObject<List<BoardShape>>(jsonString, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                return boardShapes;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error Occured: ServerCheckPointHandler:FetchCheckPoint");
                Trace.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        ///     To Get the total number of checkpoints saved at server side.
        /// </summary>
        /// <returns>Number corresponding to the total number of checkpoints at server.</returns>
        public int GetCheckpointsNumber()
        {
            return _checkpointNumber;
        }

        /// <summary>
        ///     Saves the checkpoint at the server.
        /// </summary>
        /// <param name="boardShapes">List containing all the information to save the checkpoint.</param>
        /// <param name="userId">User who requested the saving of checkpoint.</param>
        /// <returns>The number/identifier corresponding to the created checkpoint.</returns>
        public int SaveCheckpoint(List<BoardShape> boardShapes, string userId)
        {
            try
            {
                //increase the checkpoint number by one
                _checkpointNumber = _checkpointNumber + 1;

                //Added checkpoint number, userId and boardShapes in the checkpoint summery list
                _checkpointSummary.Add(
                    new Tuple<int, string, List<BoardShape>>(_checkpointNumber, userId, boardShapes));

                //Construct path from the corresopnding checkpoint number
                var boardShapesPath = _checkpointNumber + ".json";

                // Serializing boardShapes object and saving them at the boardShapesPath
                var jsonString = JsonConvert.SerializeObject(boardShapes, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                File.WriteAllText(boardShapesPath, jsonString);


                return _checkpointNumber;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error Occured: ServerCheckPointHandler:SaveCheckPoint");
                Trace.WriteLine(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     returns the list of created checkpoints and corressponding userId and boardshape
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, string, List<BoardShape>>> Summary()
        {
            return _checkpointSummary;
        }
    }
}