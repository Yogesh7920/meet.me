/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Base Class for Board Handlers
    /// </summary>
    abstract public class BoardOperationsState
    {
        /// <summary>
        /// User Level of client.
        /// </summary>
        protected int UserLevel { get; set; }

        /// <summary>
        /// state Manager instance.
        /// </summary>
        protected IClientBoardStateManagerInternal StateManager;

        /// <summary>
        /// Checker if the module is running in test mode or not
        /// </summary>
        protected static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        /// <summary>
        /// Allows setting stateManager handler when running as part of NUnit tests.
        /// </summary>
        /// <param name="stateManager">state Manager object.</param>
        public void SetStateManager(IClientBoardStateManagerInternal stateManager)
        {
            if (IsRunningFromNUnit)
            {
                StateManager = stateManager;
            }

        }

        /// <summary>
        /// Sets user level of itself and state manager.
        /// </summary>
        /// <param name="userLevel">user level.</param>
        public void SetUserLevel(int userLevel)
        {
            UserLevel = userLevel;
            StateManager.SetUserLevel(userLevel);
        }

        /// <summary>
        /// Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId);

        /// <summary>
        /// Changes the stroke color of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId);

        /// <summary>
        /// Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId);

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
        abstract public List<UXShape> CreateShape(ShapeType shapeType, Coordinate start, Coordinate end,
                                                  float strokeWidth, BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false);

        /// <summary>
        /// Perform real-time operation on shape.
        /// </summary>
        /// <param name="realTimeOperation">The RealTimeOperation to be performed.</param>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="shapeComp">Denotes whether to send the completed shape to state Manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation, Coordinate start, Coordinate end, string shapeId, DragPos dragPos, bool shapeComp = false);

        /// <summary>
        /// Delete a shape with given shape Id.
        /// </summary>
        /// <param name="shapeId">Id of the shape.</param>
        /// <returns>List of operations to be performed by UX.</returns>
        abstract public List<UXShape> Delete(string shapeId);

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> Redo();

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        abstract public List<UXShape> Undo();

        /// <summary>
        /// Gets the username for given shape Id.
        /// </summary>
        /// <param name="shapeId">Id of shape.</param>
        /// <returns>Username.</returns>
        public string GetUserName(string shapeId)
        {
            try
            {
                Trace.WriteLine("[Whiteboard] BoardOperationsState:GetUserName: Getting user for shape Id" + shapeId);
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);
                return shapeFromManager.ShapeOwnerId;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ActiveBoardOperationsHandler:Delete: Failure in getting user Name Operation.");
                Trace.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Get shape from state Manager if BoardShape with shapeId exists.
        /// </summary>
        /// <param name="shapeId">Id of shape to be fetched.</param>
        /// <returns>BoardShape returned from server, if exists, else raise exception.</returns>
        protected BoardShape GetShapeFromManager(string shapeId)
        {
            BoardShape shapeFromManager = StateManager.GetBoardShape(shapeId);
            if (shapeFromManager == null)
            {
                throw new Exception("Shape Id doesn't exist in server.");
            }
            return shapeFromManager;
        }
    }
}