/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// Handler when board is in Active State.
    /// </summary>
    class ActiveBoardOperationsHandler : BoardOperationsState
    {
        /// <summary>
        /// Class for real-time rendering for storing previous temporary object rendered on UX.
        /// </summary>
        private class LastDrawnDetails
        {
            /// <summary>
            /// Last drawn shape object.
            /// </summary>
            public BoardShape _shape;

            /// <summary>
            /// End coordinate while drawing last temporary object while real-time rendering.
            /// </summary>
            public Coordinate _end;

            /// <summary>
            /// Last real-time operation being performed.
            /// </summary>
            public RealTimeOperation _operation;

            /// <summary>
            /// Constructor for LastDrawnDetails.
            /// </summary>
            public LastDrawnDetails()
            {
                _shape = null;
                _end = null;
            }

            /// <summary>
            /// Denotes whether last real-time operation is pending.
            /// </summary>
            /// <returns></returns>
            public bool IsPending()
            {
                return _shape != null;
            }
        }

        /// <summary>
        /// Last drawn object.
        /// </summary>
        private LastDrawnDetails _lastDrawn;

        /// <summary>
        /// Constructor with initialisation.
        /// </summary>
        public ActiveBoardOperationsHandler()
        {
            _lastDrawn = new LastDrawnDetails();
            _stateManager = ClientBoardStateManager.Instance;
            UserLevel = 0;
        }

        /// <summary>
        /// Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill([NotNull] BoardColor shapeFill, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeShapeFill: Performing ChangeShapeFill Operation.");
                // get the actual BoardServer object stored in the server
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                BoardShape newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.ShapeFill = shapeFill.Clone();

                List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

                return Operations;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeShapeFill: Failure in changing Shape Fill.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Changes the stroke color of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeColor([NotNull] BoardColor strokeColor, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeStrokeColor: Performing Change StrokeColor Operation.");
                // get the actual BoardServer object stored in the server
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                BoardShape newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.StrokeColor = strokeColor.Clone();

                List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

                return Operations;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeStrokeWidth: Failure in changing Stroke Color.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, [NotNull] string shapeId)
        {
            try
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeStrokeWidth: Performing  Change StrokeWidth Operation.");
                // get the actual BoardServer object stored in the server
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                BoardShape newBoardShape = shapeFromManager.Clone();
                newBoardShape.MainShapeDefiner.StrokeWidth = strokeWidth;

                List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

                return Operations;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ChangeStrokeWidth: Failure in changing Stroke Width.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Provides Update to the Manager, after shape completion.
        /// Also yield provides the list of operations for the UX to perform.
        /// </summary>
        /// <param name="oldBoardShape">Original BoardShape.</param>
        /// <param name="newBoardShape">BoardShape after modification.</param>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        private List<UXShape> UpdateManager([NotNull] BoardShape oldBoardShape, [NotNull] BoardShape newBoardShape, Operation operationType)
        {
            // Whenever Update goes to the Manager, the last operation saved locally is discarded.
            // Set prev drawings to null in case any exists.
            _lastDrawn = null;

            // List of UXShapes to be sent to the server.
            List<UXShape> operations = new();

            string oldShapeId = oldBoardShape.Uid;

            // Creation of UXShapes for old boardshape.
            UXShape oldShape = new (UXOperation.DELETE, oldBoardShape.MainShapeDefiner, oldShapeId);

            // Creation of UXShape for new boardshape. Shape Id of the shape is kept same, After modification.
            UXShape uxNewShape = new (UXOperation.CREATE, newBoardShape.MainShapeDefiner, oldShapeId);

            // Appending them to list of operations to be performed by the UX.
            operations.Add(oldShape);
            operations.Add(uxNewShape);

            newBoardShape.RecentOperation = operationType;
            newBoardShape.LastModifiedTime = DateTime.Now;

            Trace.WriteLine("ActiveBoardOperationsHandler:UpdateManager: Sending Updates to State Manager.");

            // saving the state across clients.
            UpdateStateManager(newBoardShape);

            return operations;
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
        public override List<UXShape> CreateShape(ShapeType shapeType, [NotNull] Coordinate start, [NotNull] Coordinate end,
                                                  float strokeWidth, [NotNull] BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false)
        {
            try
            {
                // List of Operations to be send to UX
                List<UXShape> operations = new ();
                string prevShapeId;

                // A new shape creation.
                if (shapeId == null)
                {
                    _lastDrawn = null;

                    Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Initiating Creation of a new Object.");

                    // Creation and setting params for new shape
                    MainShape newMainShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);
                    newMainShape.StrokeColor = strokeColor.Clone();
                    newMainShape.StrokeWidth = strokeWidth;

                    // Creating corresponding UX shape to be sent to the UX
                    UXShape newUxShape = new (UXOperation.CREATE, newMainShape, null);
                    prevShapeId = newUxShape.WindowsShape.Uid;
                    operations.Add(newUxShape);

                    string userId = _stateManager.GetUser();
                    if (userId == null)
                    {
                        throw new Exception("Invalid User");
                    }

                    _lastDrawn = new LastDrawnDetails
                    {
                        _shape = new (newMainShape, UserLevel, DateTime.Now, DateTime.Now, prevShapeId, userId, Operation.CREATE),
                        _end = end,
                        _operation = RealTimeOperation.CREATE
                    };

                    Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Shape Creation complete.");

                }
                // check to ensure shape is same as the one previously rendering
                else if (_lastDrawn.IsPending() && _lastDrawn._shape.Uid == shapeId && _lastDrawn._operation == RealTimeOperation.CREATE)
                {
                    Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Preparing previous object for deletion");

                    // Delete the Object that was already created in the canvas because of real time rendering
                    prevShapeId = _lastDrawn._shape.Uid;
                    UXShape oldShape = new(UXOperation.DELETE, _lastDrawn._shape.MainShapeDefiner, prevShapeId);
                    operations.Add(oldShape);

                    // modify the MainshapeDefiner and also provide another reference to it.
                    MainShape modifiedPrevShape = ShapeFactory.MainShapeCreatorFactory(shapeType, _lastDrawn._end, end, _lastDrawn._shape.MainShapeDefiner);
                    UXShape newUxShape = new (UXOperation.CREATE, modifiedPrevShape, prevShapeId);
                    operations.Add(newUxShape);

                    _lastDrawn._shape.LastModifiedTime = DateTime.Now;
                    _lastDrawn._shape.RecentOperation = Operation.CREATE;

                    Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Shape updation complete.");

                }
                else
                {
                    throw new InvalidOperationException("Invalid Request paramaters for Method.");
                }

                if (shapeComp)
                {
                    Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Sending Updates to server.");

                    // send updates to server .. clone of _lastDrawn
                    BoardShape newBoardShape = _lastDrawn._shape.Clone();
                    newBoardShape.LastModifiedTime = DateTime.Now;
                    newBoardShape.CreationTime = DateTime.Now;

                    if (!_stateManager.SaveOperation(newBoardShape))
                    {
                        throw new Exception("Couldn't update state of state Manager.");
                    }

                    //reset the variables
                    _lastDrawn = null;
                }

                return operations;
            }
            catch (Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:CreateShape: Failure in creation of shape.");
                Trace.WriteLine(e.Message);
                return null;
            }
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
        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation,
                                                          [NotNull] Coordinate start, [NotNull] Coordinate end,
                                                          [NotNull] string shapeId, bool shapeComp = false)
        {
            try
            {
                // List of Operations to be send to UX
                List<UXShape> operations = new();

                if (_lastDrawn == null)
                {
                    Trace.WriteLine("ActiveBoardOperationsHandler:ModifyShapeRealTime: Initiating new modification procedure for shape.");

                    // get the actual BoardServer object stored in the server
                    BoardShape shapeFromManager = GetShapeFromManager(shapeId);
                    _lastDrawn = new LastDrawnDetails
                    {
                        _shape = shapeFromManager.Clone(),
                        _operation = realTimeOperation
                    };
                    _lastDrawn._shape.RecentOperation = Operation.MODIFY;
                    _lastDrawn._end = end;

                }
                else if (_lastDrawn.IsPending() && _lastDrawn._shape.Uid == shapeId && _lastDrawn._operation == realTimeOperation)
                {
                    Trace.WriteLine("ActiveBoardOperationsHandler:ModifyShapeRealTime: Deleting previous shape.");

                    // Append the object already rendered on local client for deletion
                    string prevShapeId = _lastDrawn._shape.Uid;
                    UXShape oldShape = new (UXOperation.DELETE, _lastDrawn._shape.MainShapeDefiner, prevShapeId);
                    operations.Add(oldShape);
                }
                else
                {
                    throw new InvalidOperationException("Invalid Request paramaters for Method.");
                }

                // modify the _lastDrawn Object
                MainShape lastDrawnMainShape = _lastDrawn._shape.MainShapeDefiner;
                switch (realTimeOperation)
                {
                    case RealTimeOperation.TRANSLATE:
                        Coordinate delta = end - _lastDrawn._end;
                        lastDrawnMainShape.Center.Add(delta);
                        break;
                    case RealTimeOperation.ROTATE:
                        lastDrawnMainShape.Rotate(_lastDrawn._end, end);
                        break;
                    default:
                        throw new Exception("The Operation " + realTimeOperation + "does not support real-time rendering.");
                }

                if (shapeComp)
                {
                    Trace.WriteLine("ActiveBoardOperationsHandler:ModifyShapeRealTime: Sending updates to state Manager.");

                    // send updates to server .. clone of _lastDrawn
                    BoardShape newBoardShape = _lastDrawn._shape.Clone();
                    newBoardShape.LastModifiedTime = DateTime.Now;
                    newBoardShape.CreationTime = DateTime.Now;
                    UpdateStateManager(newBoardShape);
                    
                    //reset the variables
                    _lastDrawn = null;
                }

                return operations;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:ModifyShapeRealTime: Failure in real time shape modification.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Perform resizing operation on shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse drag.</param>
        /// <param name="shapeId">Id of the shape.</param>
        /// <param name="dragpos">The latch used for resizing.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Resize([NotNull] Coordinate start, [NotNull] Coordinate end,
                                             [NotNull] string shapeId, [NotNull] DragPos dragpos)
        {
            try
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:Resize: Performing resize operation on shape.");
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);

                // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
                BoardShape newBoardShape = shapeFromManager.Clone();

                if (newBoardShape.MainShapeDefiner.ResizeAboutCenter(start, end, dragpos))
                {
                    List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);
                    return Operations;
                }
                return new List<UXShape>();
            }
            catch (Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:Resize: Failure in real time shape modification.");
                Trace.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Send update to state Manager, and verify its response.
        /// </summary>
        /// <param name="boardShape">Shape to update in stateManager.</param>
        private void UpdateStateManager(BoardShape boardShape)
        {
            if (!_stateManager.SaveOperation(boardShape))
            {
                throw new Exception("Couldn't update state of state Manager.");
            }
        }

        /// <summary>
        /// Delete a shape with given shape Id.
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
                BoardShape shapeFromManager = GetShapeFromManager(shapeId);

                UXShape oldShape = new (UXOperation.DELETE, shapeFromManager.MainShapeDefiner, shapeFromManager.Uid);
                operations.Add(oldShape);

                BoardShape shapeFromManagerClone = shapeFromManager.Clone();
                shapeFromManagerClone.RecentOperation = Operation.DELETE;
                shapeFromManagerClone.LastModifiedTime = DateTime.Now;

                UpdateStateManager(shapeFromManagerClone);

                _lastDrawn = null;
                return operations;
            }
            catch(Exception e)
            {
                Trace.WriteLine("ActiveBoardOperationsHandler:Delete: Failure in Resize Operation.");
                Trace.WriteLine(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Perform Undo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Undo()
        {
            return _stateManager.DoUndo();
        }

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Redo()
        {
            return _stateManager.DoRedo();
        }
    }
}
