/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/26/2021
 * Date Modified: 11/26/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public class UXShapeHelper
    {
        public UXOperation UxOperation { get; set; }

        public MainShape MainShapeDefiner { get; set; }

        public string ShapeId { get; set; }

        public int CheckpointNumber { get; set; }

        public Operation OperationType { get; set; }

        public UXShapeHelper([NotNull] UXOperation uXOperation, [NotNull] MainShape mainShape, string shapeId = null, int checkpointNumber = 0, Operation operationType = Operation.NONE)
        {
            UxOperation = uXOperation;
            MainShapeDefiner = mainShape;
            ShapeId = shapeId;
            CheckpointNumber = checkpointNumber;
            OperationType = operationType;
        }

        public UXShapeHelper(int checkpointNumber, Operation operationFlag = Operation.FETCH_CHECKPOINT)
        {
            UxOperation = UXOperation.NONE;
            MainShapeDefiner = null;
            ShapeId = null;
            CheckpointNumber = checkpointNumber;
            OperationType = operationFlag;
        }

        public UXShapeHelper()
        {

        }
    }
}
