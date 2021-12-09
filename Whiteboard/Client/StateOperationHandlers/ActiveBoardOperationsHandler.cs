/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Whiteboard
{
    /// <summary>
    ///     Handler when board is in Active State.
    /// </summary>
    public class ActiveBoardOperationsHandler : BoardOperationsState
    {
        /// <summary>
        ///     Last drawn object.
        /// </summary>
        private LastDrawnDetails _lastDrawn;

        /// <summary>
        ///     Constructor with initialisation.
        /// </summary>
        public ActiveBoardOperationsHandler()
        {
            _lastDrawn = null;
            StateManager = ClientBoardStateManager.Instance;
            UserLevel = 0;
        }

        /// <summary>
        ///     Getting the current lastDrawn Object value.
        ///     Used in testing.
        /// </summary>
        /// <returns></returns>
        public BoardShape GetLastDrawn()
        {
            if (IsRunningFromNUnit) return _lastDrawn?.LastShape;
            return null;
        }

        /// <summary>
        ///     Setting the lastDrawn object.
        ///     Used for testing.
        /// </summary>
        /// <param name="shape">Last drawn boardshape.</param>
        /// <param name="end">Last Operation's mouse-up coordinate.</param>
        /// <param name="operation">Last performed operations.</param>
        public void SetLastDrawn(BoardShape shape, Coordinate end = null,
            RealTimeOperation operation = RealTimeOperation.Rotate)
        {
            if (IsRunningFromNUnit)
            {
                if (shape == null)
                    _lastDrawn = null;
                else
                    _lastDrawn = new LastDrawnDetails
                    {
                        LastShape = shape,
                        End = end,
                        LastOperation = operation
                    };
            }
        }

        /// <summary>
        ///     Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill([NotNull] BoardColor shapeFill, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeShapeFill: Performing ChangeShapeFill Operation.");
                // get the actual BoardServer object stored in the server
                var shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                var newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.ShapeFill = shapeFill.Clone();

                var Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.Modify);

                return Operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeShapeFill: Failure in changing Shape Fill.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Changes the stroke color of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeColor([NotNull] BoardColor strokeColor, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeStrokeColor: Performing Change StrokeColor Operation.");
                // get the actual BoardServer object stored in the server
                var shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                var newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.StrokeColor = strokeColor.Clone();

                var Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.Modify);

                return Operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeStrokeWidth: Failure in changing Stroke Color.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeStrokeWidth: Performing  Change StrokeWidth Operation.");
                // get the actual BoardServer object stored in the server
                var shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                var newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.StrokeWidth = strokeWidth;

                var Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.Modify);

                return Operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ChangeStrokeWidth: Failure in changing Stroke Width.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Provides Update to the Manager, after shape completion.
        ///     Also yield provides the list of operations for the UX to perform.
        /// </summary>
        /// <param name="oldBoardShape">Original BoardShape.</param>
        /// <param name="newBoardShape">BoardShape after modification.</param>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        private List<UXShape> UpdateManager([NotNull] BoardShape oldBoardShape, [NotNull] BoardShape newBoardShape,
            Operation operationType)
        {
            // Whenever Update goes to the Manager, the last operation saved locally is discarded.
            // Set prev drawings to null in case any exists.
            _lastDrawn = null;

            // List of UXShapes to be sent to the server.
            List<UXShape> operations = new();

            var oldShapeId = oldBoardShape.Uid;

            // Creation of UXShapes for old boardshape.
            UXShape oldShape = new(UXOperation.Delete, oldBoardShape.MainShapeDefiner, oldShapeId);

            // Creation of UXShape for new boardshape. Shape Id of the shape is kept same, After modification.
            UXShape uxNewShape = new(UXOperation.Create, newBoardShape.MainShapeDefiner, oldShapeId);

            // Appending them to list of operations to be performed by the UX.
            operations.Add(oldShape);
            operations.Add(uxNewShape);

            newBoardShape.RecentOperation = operationType;
            newBoardShape.LastModifiedTime = DateTime.Now;

            Trace.WriteLine(
                "[Whiteboard] ActiveBoardOperationsHandler:UpdateManager: Sending Updates to State Manager.");

            // saving the state across clients.
            UpdateStateManager(newBoardShape);

            return operations;
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
        public override List<UXShape> CreateShape(ShapeType shapeType, [NotNull] Coordinate start,
            [NotNull] Coordinate end,
            float strokeWidth, [NotNull] BoardColor strokeColor, string shapeId = null,
            bool shapeComp = false)
        {
            try
            {
                // List of Operations to be send to UX
                List<UXShape> operations = new();
                string prevShapeId;
                var alreadyDrawn = false;

                // A new shape creation.
                if (shapeId == null)
                {
                    _lastDrawn = null;

                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:CreateShape: Initiating Creation of a new Object.");

                    // Creation and setting params for new shape
                    var newMainShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);
                    newMainShape.StrokeColor = strokeColor.Clone();
                    newMainShape.StrokeWidth = strokeWidth;

                    // Creating corresponding UX shape to be sent to the UX
                    UXShape newUxShape = new(UXOperation.Create, newMainShape);
                    prevShapeId = newUxShape.WindowsShape.Uid;
                    operations.Add(newUxShape);

                    var userId = StateManager.GetUser();

                    if (userId == null) throw new Exception("Invalid User");

                    _lastDrawn = new LastDrawnDetails
                    {
                        LastShape = new BoardShape(newMainShape, UserLevel, DateTime.Now, DateTime.Now, prevShapeId,
                            userId, Operation.Create),
                        End = end,
                        LastOperation = RealTimeOperation.Create
                    };

                    Trace.WriteLine("[Whiteboard] ActiveBoardOperationsHandler:CreateShape: Shape Creation complete.");
                }
                // check to ensure shape is same as the one previously rendering
                else if (_lastDrawn.IsPending() && _lastDrawn.LastShape.Uid == shapeId &&
                         _lastDrawn.LastOperation == RealTimeOperation.Create)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:CreateShape: Preparing previous object for deletion");
                    alreadyDrawn = true;

                    // Delete the Object that was already created in the canvas because of real time rendering
                    prevShapeId = _lastDrawn.LastShape.Uid;
                    UXShape oldShape = new(UXOperation.Delete, _lastDrawn.LastShape.MainShapeDefiner, prevShapeId);
                    operations.Add(oldShape);

                    // modify the MainshapeDefiner and also provide another reference to it.
                    var modifiedPrevShape = ShapeFactory.MainShapeCreatorFactory(shapeType, _lastDrawn.End, end,
                        _lastDrawn.LastShape.MainShapeDefiner);
                    UXShape newUxShape = new(UXOperation.Create, modifiedPrevShape, prevShapeId);
                    operations.Add(newUxShape);

                    _lastDrawn.LastShape.LastModifiedTime = DateTime.Now;
                    _lastDrawn.LastShape.RecentOperation = Operation.Create;
                    _lastDrawn.End = end.Clone();

                    Trace.WriteLine("[Whiteboard] ActiveBoardOperationsHandler:CreateShape: Shape updation complete.");
                }
                else
                {
                    throw new InvalidOperationException("Invalid Request paramaters for Method.");
                }

                if (shapeComp)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:CreateShape: Sending Updates to server.");

                    // send updates to server .. clone of _lastDrawn
                    var newBoardShape = _lastDrawn.LastShape.Clone();
                    newBoardShape.LastModifiedTime = DateTime.Now;
                    newBoardShape.CreationTime = DateTime.Now;

                    if (!StateManager.SaveOperation(newBoardShape))
                    {
                        _lastDrawn = null;
                        return UndoRealTimeRenderingCreation(operations, alreadyDrawn, true);
                    }

                    //reset the variables
                    _lastDrawn = null;
                }

                return operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] CreateShape: Failure in creation of shape.");
                Trace.WriteLine(e.Message);
                return null;
            }
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
        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation,
            [NotNull] Coordinate start, [NotNull] Coordinate end,
            [NotNull] string shapeId, DragPos dragpos, bool shapeComp = false)
        {
            try
            {
                // List of Operations to be send to UX
                List<UXShape> operations = new();

                if (_lastDrawn == null)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:ModifyShapeRealTime: Initiating new modification procedure for shape.");

                    // get the actual BoardServer object stored in the server
                    var shapeFromManager = GetShapeFromManager(shapeId);
                    _lastDrawn = new LastDrawnDetails
                    {
                        LastShape = shapeFromManager.Clone(),
                        LastOperation = realTimeOperation,
                        End = start
                    };
                    _lastDrawn.LastShape.RecentOperation = Operation.Modify;
                }
                else if (_lastDrawn.IsPending() && _lastDrawn.LastShape.Uid == shapeId &&
                         _lastDrawn.LastOperation == realTimeOperation)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:ModifyShapeRealTime: Deleting previous temporary shape.");
                }
                else
                {
                    throw new InvalidOperationException("Invalid Request paramaters for Method.");
                }

                // Append the object already rendered on local client for deletion
                var prevShapeId = _lastDrawn.LastShape.Uid;
                UXShape oldShape = new(UXOperation.Delete, _lastDrawn.LastShape.MainShapeDefiner, prevShapeId);
                operations.Add(oldShape);


                // modify the _lastDrawn Object
                var lastDrawnMainShape = _lastDrawn.LastShape.MainShapeDefiner;

                // indicative of the success of operation
                var operationSuccess = false;
                switch (realTimeOperation)
                {
                    case RealTimeOperation.Translate:

                        var delta = end - _lastDrawn.End;
                        lastDrawnMainShape.Center.Add(delta);
                        lastDrawnMainShape.Start.Add(delta);
                        operationSuccess = true;
                        break;
                    case RealTimeOperation.Rotate:
                        operationSuccess = lastDrawnMainShape.Rotate(_lastDrawn.End, end);
                        break;
                    case RealTimeOperation.Resize:
                        operationSuccess = lastDrawnMainShape.ResizeAboutCenter(_lastDrawn.End, end, dragpos);
                        break;
                    case RealTimeOperation.Create:
                        throw new Exception(
                            "Create Operation Real Time Handling not performed by this function. Call CreateShape");
                    default:
                        throw new Exception("The Operation " + realTimeOperation +
                                            "does not support real-time rendering.");
                }

                // If the modification succeeds, then add that for UX to render.
                if (operationSuccess)
                {
                    UXShape newUxShape = new(UXOperation.Create, lastDrawnMainShape, shapeId);
                    operations.Add(newUxShape);
                    _lastDrawn.End = end;
                }

                if (shapeComp)
                {
                    Trace.WriteLine(
                        "[Whiteboard] ActiveBoardOperationsHandler:ModifyShapeRealTime: Sending updates to state Manager.");

                    // send updates to server .. clone of _lastDrawn
                    var newBoardShape = _lastDrawn.LastShape.Clone();
                    newBoardShape.LastModifiedTime = DateTime.Now;

                    if (!UserLevelHandler.IsAccessible(UserLevel, _lastDrawn.LastShape.UserLevel) ||
                        !StateManager.SaveOperation(newBoardShape))
                    {
                        _lastDrawn = null;
                        return UndoRealTimeRenderingModify(shapeId, operations);
                    }

                    //reset the variables
                    _lastDrawn = null;
                }

                Console.WriteLine(operations.Count);

                return operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    "[Whiteboard] ActiveBoardOperationsHandler:ModifyShapeRealTime: Failure in real time shape modification.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Returns appropriate output to UX in case the state Manager fails to update its state for modification operation.
        /// </summary>
        /// <param name="uid">uid of the shape.</param>
        /// <param name="operations">The list of UX operations, to be sent to the UX in case server update had succeeded.</param>
        /// <returns>Modified list of operations.</returns>
        private List<UXShape> UndoRealTimeRenderingModify(string uid, List<UXShape> operations)
        {
            Trace.WriteLine(
                "[Whiteboard] ActiveBoardOperationsHandler:UndoRealTimeRenderingModify: Couldn't send updates to the server. Removing Temporary Rendering.");

            // get the shape from manager in order to re-render the original shape distorted by UX.
            var shapeFromManager = GetShapeFromManager(uid);
            UXShape oldShape = new(UXOperation.Create, shapeFromManager.MainShapeDefiner, uid);
            operations.RemoveAt(1);
            operations.Add(oldShape);
            return operations;
        }

        /// <summary>
        ///     Returns appropriate output to UX in case the state Manager fails to update its state for creation operation.
        /// </summary>
        /// <param name="operations">The list of UX operations, to be sent to the UX in case server update had succeeded.</param>
        /// <param name="alreadyDrawn">Indicative if some real-time rendering happened from whiteBoard handler.</param>
        /// <param name="uxSpecific">Specific requirement of UX based on their way of implementation.</param>
        /// <returns>Modified list of operations.</returns>
        private static List<UXShape> UndoRealTimeRenderingCreation(List<UXShape> operations, bool alreadyDrawn,
            bool uxSpecific)
        {
            Trace.WriteLine(
                "[Whiteboard] ActiveBoardOperationsHandler:UndoRealTimeRenderingCreation: Couldn't send updates to the server. Removing Temporary Rendering.");

            // UX might be doing real-time handling on its own.. so this flag specifies whether to take into consideration.
            if (uxSpecific)
            {
                return null;
            }

            if (alreadyDrawn && !uxSpecific)
            {
                operations.RemoveAt(1);
                return operations;
            }

            return null;
        }

        /// <summary>
        ///     Send update to state Manager, and verify its response.
        /// </summary>
        /// <param name="boardShape">Shape to update in stateManager.</param>
        private void UpdateStateManager(BoardShape boardShape)
        {
            if (!UserLevelHandler.IsAccessible(UserLevel, boardShape.UserLevel) ||
                !StateManager.SaveOperation(boardShape)) throw new Exception("Couldn't update state of state Manager.");
        }

        /// <summary>
        ///     Delete a shape with given shape Id.
        /// </summary>
        /// <param name="shapeId">Id of the shape.</param>
        /// <returns>List of operations to be performed by UX.</returns>
        public override List<UXShape> Delete([NotNull] string shapeId)
        {
            try
            {
                // List of Operations to be send to UX
                List<UXShape> operations = new();

                // get the actual BoardServer object stored in the server
                var shapeFromManager = GetShapeFromManager(shapeId);

                UXShape oldShape = new(UXOperation.Delete, shapeFromManager.MainShapeDefiner, shapeFromManager.Uid);
                operations.Add(oldShape);

                // set params to send to state manager.
                var shapeFromManagerClone = shapeFromManager.Clone();
                shapeFromManagerClone.RecentOperation = Operation.Delete;
                shapeFromManagerClone.LastModifiedTime = DateTime.Now;

                UpdateStateManager(shapeFromManagerClone);

                // reset the lastDrawn so that it does not hinder with the next operations.
                _lastDrawn = null;
                return operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Whiteboard] ActiveBoardOperationsHandler:Delete: Failure in Resize Operation.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Perform Undo Operation.
        /// </summary>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Undo()
        {
            return StateManager.DoUndo();
        }

        /// <summary>
        ///     Perform Redo Operation.
        /// </summary>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Redo()
        {
            return StateManager.DoRedo();
        }

        /// <summary>
        ///     Class for real-time rendering for storing previous temporary object rendered on UX.
        /// </summary>
        private class LastDrawnDetails
        {
            /// <summary>
            ///     End coordinate while drawing last temporary object while real-time rendering.
            /// </summary>
            public Coordinate End;

            /// <summary>
            ///     Last real-time operation being performed.
            /// </summary>
            public RealTimeOperation LastOperation;

            /// <summary>
            ///     Last drawn shape object.
            /// </summary>
            public BoardShape LastShape;

            /// <summary>
            ///     Constructor for LastDrawnDetails.
            /// </summary>
            public LastDrawnDetails()
            {
                LastShape = null;
                End = null;
            }

            /// <summary>
            ///     Denotes whether last real-time operation is pending.
            /// </summary>
            /// <returns>Bool indicating whether it is continuation of previous modification/creation.</returns>
            public bool IsPending()
            {
                return LastShape != null;
            }
        }
    }
}