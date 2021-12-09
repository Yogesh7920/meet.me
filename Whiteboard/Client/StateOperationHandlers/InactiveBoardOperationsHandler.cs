/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Whiteboard
{
    /// <summary>
    ///     Handler when Board is Inactive State.
    /// </summary>
    public class InactiveBoardOperationsHandler : BoardOperationsState
    {
        /// <summary>
        ///     Constructor for InactiveBoardOperationsHandler
        /// </summary>
        public InactiveBoardOperationsHandler()
        {
            UserLevel = 0;
            StateManager = ClientBoardStateManager.Instance;
        }

        /// <summary>
        ///     Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Changes the stroke color of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Creates shape based on mouse drag.
        /// </summary>
        /// <param name="shapeType">Denotes which shape to create.</param>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="strokeWidth">Width of the outline stroke.</param>
        /// <param name="strokeColor">Color of the outline stroke.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="shapeComp">Denotes whether to send the completed shape to state Manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> CreateShape(ShapeType shapetype, Coordinate start, Coordinate end,
            float strokeWidth, BoardColor strokeColor, string shapeId = null,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Perform real-time operation on shape.
        /// </summary>
        /// <param name="realTimeOperation">The RealTimeOperation to be performed.</param>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="shapeComp">Denotes whether to send the completed shape to state Manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation, Coordinate start,
            Coordinate end, string shapeId, DragPos dragpos, bool shapeComp = false)
        {
            try
            {
                Trace.WriteLine(
                    "[Whiteboard] InactiveBoardOperationsHandler:ModifyShapeRealTime: Sending original shape stored in Manager to re-render");

                // This requirement is very specified to the UX team.
                var shapeFromManager = GetShapeFromManager(shapeId);
                UXShape oldShape = new(UXOperation.Delete, shapeFromManager.MainShapeDefiner, shapeId);
                UXShape newShape = new(UXOperation.Create, shapeFromManager.MainShapeDefiner, shapeId);

                List<UXShape> grey = new() {oldShape, newShape};

                Trace.WriteLine(
                    "[Whiteboard] InactiveBoardOperationsHandler:ModifyShapeRealTime: Original Shape sent!");
                return grey;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
        }


        /// <summary>
        ///     Delete a shape with given shape Id.
        /// </summary>
        /// <param name="shapeId">Id of the shape.</param>
        /// <returns>List of operations to be performed by UX.</returns>
        public override List<UXShape> Delete(string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Perform Redo Operation.
        /// </summary>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Undo()
        {
            return new List<UXShape>();
        }

        /// <summary>
        ///     Perform Redo Operation.
        /// </summary>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Redo()
        {
            return new List<UXShape>();
        }
    }
}