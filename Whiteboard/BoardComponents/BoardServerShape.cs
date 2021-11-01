/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 11/01/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public class BoardServerShape
    {
        public List<BoardShape> ShapeUpdates { get; set; }
        public Operation OperationFlag { get; set; }
        public string RequesterId { get; set; }
        public DateTime RequestTime { get; set; }
        public int CheckpointNumber { get; set; }

        public BoardServerShape(List<BoardShape> shapeUpdates, Operation operation, string requesterId, int checkpointNumber = 0)
        {
            ShapeUpdates = shapeUpdates;
            OperationFlag = operation;
            RequesterId = requesterId;
            CheckpointNumber = checkpointNumber;
            RequestTime = DateTime.Now;
        }

    }
}