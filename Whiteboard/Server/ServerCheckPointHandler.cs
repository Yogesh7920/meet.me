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
using Newtonsoft;
using Newtonsoft.Json;
using System.Text.Json;



namespace Whiteboard
{
    internal class ServerCheckPointHandler : IServerCheckPointHandler
    {
        private int CheckpointNumber = 0;

        //List of checkpoints to keep track of checkpoint number and corressponding userId and boardShape
        private readonly List<Tuple<int, String, List<BoardShape>>> CheckpointSummary = new List<Tuple<int, String, List<BoardShape>>>();

        /// <summary>
        /// Fetches the checkpoint corresponding to the checkPointNumber
        /// </summary>
        /// <param name="checkpointNumber">The identifier/number of the checkpoint which needs to fetched.</param>
        /// <returns>Returns list of BoardShape summarzing the checkpoint to the ServerBoardStateManager.</returns>
        public List<BoardShape> FetchCheckpoint(int checkpointNumber)
        {
            try
            {   // Construct path from the checkpointNumber
                string boardShapesPath = checkpointNumber.ToString() + ".json";

                // Get the file corresponding to the path
                /*StreamReader streamReader = new StreamReader(boardShapesPath);

                //Deserializing the file
                XmlSerializer xml = new XmlSerializer(typeof(List<BoardShape>));
                var boardShapes = (List<BoardShape>)xml.Deserialize(streamReader);*/

                string jsonString = File.ReadAllText(boardShapesPath);
                //List<BoardShape> boardShapes = JsonSerializer.Deserialize<List<BoardShape>>(jsonString);
                List<BoardShape> boardShapes = JsonConvert.DeserializeObject<List<BoardShape>> (jsonString, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                return boardShapes;

            }
            catch (Exception e)
            {
                Trace.WriteLine("invalid checkpointNumber");
                Trace.WriteLine(e.Message);
            }
            return null;

        }

        /// <summary>
        /// To Get the total number of checkpoints saved at server side.
        /// </summary>
        /// <returns>Number corresponding to the total number of checkpoints at server.</returns>
        public int GetCheckpointsNumber()
        {
            return CheckpointNumber;
        }

        /// <summary>
        /// Saves the checkpoint at the server. 
        /// </summary>
        /// <param name="boardShapes">List containing all the information to save the checkpoint.</param>
        /// <param name="userId">User who requested the saving of checkpoint.</param>
        /// <returns>The number/identifier corresponding to the created checkpoint.</returns>
        public int SaveCheckpoint(List<BoardShape> boardShapes, string userId)
        {
            //increase the checkpoint number by one
            CheckpointNumber = CheckpointNumber + 1;

            //Added checkpoint number, userId and boardShapes in the checkpoint summery list
            CheckpointSummary.Add(new Tuple<int, string, List<BoardShape>>(CheckpointNumber, userId, boardShapes));

            //Construct path from the corresopnding checkpoint number
            string boardShapesPath = CheckpointNumber.ToString() + ".json";

            // Serializing boardShapes object and saving them at the boardShapesPath
            /*XmlSerializer xml = new XmlSerializer(boardShapes.GetType());
            StreamWriter streamWriter = new StreamWriter(boardShapesPath);

            xml.Serialize(streamWriter, boardShapes);

            streamWriter.Close();
            string jsonString = JsonSerializer.Serialize(boardShapes);
            File.WriteAllText(boardShapesPath, jsonString);*/
            string jsonString = JsonConvert.SerializeObject(boardShapes, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(boardShapesPath, jsonString); 


            return CheckpointNumber;
        }
        /// <summary>
        /// returns the list of created checkpoints and corressponding userId and boardshape
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, String, List<BoardShape>>> Summary()
        {
            return CheckpointSummary;
        }
    }


}
