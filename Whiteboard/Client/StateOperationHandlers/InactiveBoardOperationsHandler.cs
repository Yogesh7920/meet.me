/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/12/2021
**/

using System.Collections.Generic;

namespace Whiteboard
{
    /// <summary>
    /// Handler when Board is Inactive State.
    /// </summary>
    class InactiveBoardOperationsHandler : BoardOperationsState
    {
        /// <summary>
        /// Constructor for InactiveBoardOperationsHandler
        /// </summary>
        public InactiveBoardOperationsHandler()
        {
            UserLevel = 0;
        }

        /// <summary>
        /// Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Changes the stroke color of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Creates shape based on mouse drag.
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
        /// Perform real-time operation on shape.
        /// </summary>
        /// <param name="realTimeOperation">The RealTimeOperation to be performed.</param>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="shapeComp">Denotes whether to send the completed shape to state Manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation, Coordinate start,
                                                          Coordinate end, string shapeId, bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Perform resizing operation on shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="dragPos">The latch used for performing resizing.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Resize(Coordinate start, Coordinate end, string shapeId, DragPos dragPos)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Delete a shape with given shape Id.
        /// </summary>
        /// <param name="shapeId">Id of the shape.</param>
        /// <returns>List of operations to be performed by UX.</returns>
        public override List<UXShape> Delete(string shapeId)
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Undo()
        {
            return new List<UXShape>();
        }

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Redo()
        {
            return new List<UXShape>();
        }
    }
}
