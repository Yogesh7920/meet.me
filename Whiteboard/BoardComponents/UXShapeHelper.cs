/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/26/2021
 * Date Modified: 11/26/2021
**/

using System.Diagnostics.CodeAnalysis;

namespace Whiteboard
{
    /// <summary>
    ///     Helper class to store mid-way elements.
    /// </summary>
    public class UXShapeHelper
    {
        /// <summary>
        ///     The parametrized constructor.
        /// </summary>
        /// <param name="uXOperation">The operation UX will perform on UXShape.</param>
        /// <param name="mainShape">Main Shape</param>
        /// <param name="shapeId">The Uid of the shape.</param>
        /// <param name="checkpointNumber">The number of checkpoints at server.</param>
        /// <param name="operationType">The type of operation send to server.</param>
        public UXShapeHelper([NotNull] UXOperation uXOperation, [NotNull] MainShape mainShape, string shapeId = null,
            int checkpointNumber = 0, Operation operationType = Operation.None)
        {
            UxOperation = uXOperation;
            MainShapeDefiner = mainShape;
            ShapeId = shapeId;
            CheckpointNumber = checkpointNumber;
            OperationType = operationType;
        }

        /// <summary>
        ///     The parametrized constructor.
        /// </summary>
        /// <param name="checkpointNumber">The number of checkpoints at server.</param>
        /// <param name="operationFlag">The type of operation send to server.</param>
        public UXShapeHelper(int checkpointNumber, Operation operationFlag = Operation.FetchCheckpoint)
        {
            UxOperation = UXOperation.None;
            MainShapeDefiner = null;
            ShapeId = null;
            CheckpointNumber = checkpointNumber;
            OperationType = operationFlag;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public UXShapeHelper()
        {
        }

        /// <summary>
        ///     The operation UX will perform on UXShape.
        /// </summary>
        public UXOperation UxOperation { get; set; }

        /// <summary>
        ///     Main Shape
        /// </summary>
        public MainShape MainShapeDefiner { get; set; }

        /// <summary>
        ///     The Uid of the shape.
        /// </summary>
        public string ShapeId { get; set; }

        /// <summary>
        ///     The number of checkpoints at server.
        /// </summary>
        public int CheckpointNumber { get; set; }

        /// <summary>
        ///     The type of operation send to server.
        /// </summary>
        public Operation OperationType { get; set; }
    }
}