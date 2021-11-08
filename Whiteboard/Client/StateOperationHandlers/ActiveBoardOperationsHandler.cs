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

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public enum RealTimeOperation
    {
        TRANSLATE,
        ROTATE,
        RESIZE_HEIGHT,
        RESIZE_WIDTH,
        CREATE,
        RESIZE
    }
    class ActiveBoardOperationsHandler : BoardOperationsState
    {

        private class _lastDrawnDetails{
            public BoardShape _shape;
            public Coordinate _end;
            public RealTimeOperation _operation;

            public _lastDrawnDetails()
            {
                _shape = null;
                _end = null;
            }

            public bool isPending()
            {
                return (_shape == null) ? false : true;
            }
        }

        private _lastDrawnDetails _lastDrawn;

        public ActiveBoardOperationsHandler()
        {
            _lastDrawn = new _lastDrawnDetails();
            _stateManager = ClientBoardStateManager.Instance;
            UserLevel = 0;
        }
        
        //public delete - to implemented

        /// <summary>
        /// Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill([NotNull]BoardColor shapeFill, [NotNull]string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);
            if (shapeFromManager == null) 
            {
                Trace.WriteLine("Invalid Shape Id: Shape Id provided for ChangeShapeFill does not exist in State Manager.");
                return new List<UXShape>();
            }

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape newBoardShape = shapeFromManager.Clone();
            newBoardShape.MainShapeDefiner.ShapeFill = shapeFill.Clone();

            List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

            return Operations;
        }

        /// <summary>
        /// Changes the stroke of the shape.
        /// </summary>
        /// <param name="strokeColor">Modified fill color of outline stroke of shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeColor([NotNull] BoardColor strokeColor, [NotNull] string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);
            if (shapeFromManager == null)
            {
                Trace.WriteLine("Invalid Shape Id: Shape Id provided for ChangeShapeFill does not exist in State Manager.");
                return new List<UXShape>();
            }

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape newBoardShape = shapeFromManager.Clone();
            newBoardShape.MainShapeDefiner.StrokeColor = strokeColor.Clone();

            List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

            return Operations;
        }

        /// <summary>
        /// Changes the stroke Width.
        /// </summary>
        /// <param name="strokeWidth">Width of stroke.</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, [NotNull] string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);
            if (shapeFromManager == null)
            {
                Trace.WriteLine("Invalid Shape Id: Shape Id provided for ChangeShapeFill does not exist in State Manager.");
                return new List<UXShape>();
            }

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape newBoardShape = shapeFromManager.Clone();
            newBoardShape.MainShapeDefiner.StrokeWidth = strokeWidth;

            List<UXShape> Operations = UpdateManager(shapeFromManager, newBoardShape, Operation.MODIFY);

            return Operations;
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
            List<UXShape> operations = new List<UXShape>();

            // Shape Id of the shape is kept same, After modification.
            string oldShapeId = oldBoardShape.ShapeOwnerId;

            // Creation of UXShapes for old boardshape.
            UXShape oldShape = new UXShape(UXOperation.DELETE, oldBoardShape.MainShapeDefiner, oldShapeId);

            // Creation of UXShape for new boardshape.
            UXShape uxNewShape = new UXShape(UXOperation.CREATE, newBoardShape.MainShapeDefiner, oldShapeId);

            // Appending them to list of operations to be performed by the UX.
            operations.Add(oldShape);
            operations.Add(uxNewShape);

            // Setting flags in NewBoardShape before sending to Manager.
            newBoardShape.RecentOperation = operationType;
            newBoardShape.LastModifiedTime = DateTime.Now;

            // saving the state across clients.
            _stateManager.SaveOperation(newBoardShape);

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
        /// <param name="shapeComp">Denotes whether the shape is complete, or if it is for real-time rendering.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> CreateShape(ShapeType shapeType, [NotNull] Coordinate start, [NotNull] Coordinate end,
                                                  float strokeWidth, [NotNull] BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false)
        {

            // List of Operations to be send to UX
            List<UXShape> operations = new List<UXShape>();
            string prevShapeId;

            // A new shape creation.
            if (shapeId == null)
            {
                _lastDrawn = null;

                // Creation and setting params for new shape
                MainShape newMainShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);
                newMainShape.StrokeColor = strokeColor.Clone();
                newMainShape.StrokeWidth = strokeWidth;

                // Creating corresponding UX shape to be sent to the UX
                UXShape newUxShape = new UXShape(UXOperation.CREATE, newMainShape, null);
                prevShapeId = newUxShape.WindowsShape.Uid;
                operations.Add(newUxShape);

                string userId = _stateManager.GetUser();

                _lastDrawn = new _lastDrawnDetails();
                _lastDrawn._shape = new BoardShape(newMainShape, UserLevel, DateTime.Now, DateTime.Now, prevShapeId, userId, Operation.CREATE);
                _lastDrawn._end = end;
                _lastDrawn._operation = RealTimeOperation.CREATE;

            }
            // check to ensure shape is same as the one previously rendering
            else if (_lastDrawn.isPending() && _lastDrawn._shape.Uid == shapeId && _lastDrawn._operation == RealTimeOperation.CREATE)
            {

                // Delete the Object that was already created in the canvas because of real time rendering
                prevShapeId = _lastDrawn._shape.Uid;
                UXShape oldShape = new UXShape(UXOperation.DELETE, _lastDrawn._shape.MainShapeDefiner, prevShapeId);
                operations.Add(oldShape);

                // modify the MainshapeDefiner and also provide another reference to it.
                MainShape modifiedPrevShape = ShapeFactory.MainShapeCreatorFactory(shapeType, _lastDrawn._end, end, _lastDrawn._shape.MainShapeDefiner);
                UXShape newUxShape = new UXShape(UXOperation.CREATE, modifiedPrevShape, prevShapeId);
                operations.Add(newUxShape);

                _lastDrawn._shape.LastModifiedTime = DateTime.Now;
                _lastDrawn._shape.RecentOperation = Operation.CREATE;

            }
            else
            {
                // throw exception
            }

            if (shapeComp)
            {
                // send updates to server .. clone of _lastDrawn
                BoardShape newBoardShape = _lastDrawn._shape.Clone();
                newBoardShape.LastModifiedTime = DateTime.Now;
                newBoardShape.CreationTime = DateTime.Now;
                _stateManager.SaveOperation(newBoardShape);

                //reset the variables
                _lastDrawn = null;
            }

            return operations;

        }

        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation,
                                                          [NotNull] Coordinate start, [NotNull] Coordinate end,
                                                          [NotNull] string shapeId, bool shapeComp = false)
        {

            // List of Operations to be send to UX
            List<UXShape> operations = new List<UXShape>();

            if (_lastDrawn == null)
            {
                // get the actual BoardServer object stored in the server
                BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);
                if (shapeFromManager == null)
                {
                    Trace.WriteLine("Invalid Shape Id: Shape Id provided for Modification of Shape does not exist in State Manager.");
                    return new List<UXShape>();
                }
                _lastDrawn = new _lastDrawnDetails();
                _lastDrawn._shape = shapeFromManager.Clone();
                _lastDrawn._operation = realTimeOperation;
                _lastDrawn._shape.RecentOperation = Operation.MODIFY;
                _lastDrawn._end = end;
            }
            else if (_lastDrawn.isPending() && _lastDrawn._shape.Uid == shapeId && _lastDrawn._operation == realTimeOperation)
            {
                // Append the object already rendered on local client for deletion
                string prevShapeId = _lastDrawn._shape.Uid;
                UXShape oldShape = new UXShape(UXOperation.DELETE, _lastDrawn._shape.MainShapeDefiner, prevShapeId);
                operations.Add(oldShape);
            }
            else
            {
                // throw an error
            }

            // modify the _lastDrawn Object
            MainShape lastDrawnMainShape = _lastDrawn._shape.MainShapeDefiner;
            if (realTimeOperation == RealTimeOperation.TRANSLATE)
            {
                Coordinate delta = end - _lastDrawn._end;
                lastDrawnMainShape.Center.Add(delta);
                lastDrawnMainShape.Start.Add(delta);

            }
            else if (realTimeOperation == RealTimeOperation.ROTATE)
            {
                Coordinate v1 = _lastDrawn._end - lastDrawnMainShape.Center;
                Coordinate v2 = end - lastDrawnMainShape.Center;
                float rotAngle = (float)Math.Atan2(v2.C - v1.C, v2.R - v1.R);
                lastDrawnMainShape.AngleOfRotation += rotAngle;
            }
            else if (realTimeOperation == RealTimeOperation.RESIZE)
            {

            }
            else if (realTimeOperation == RealTimeOperation.RESIZE_HEIGHT)
            {

            }
            else if (realTimeOperation == RealTimeOperation.RESIZE_WIDTH)
            {

            }
            else
            {
                // throw exception
            }
            

            if (shapeComp)
            {
                // send updates to server .. clone of _lastDrawn
                BoardShape newBoardShape = _lastDrawn._shape.Clone();
                newBoardShape.LastModifiedTime = DateTime.Now;
                newBoardShape.CreationTime = DateTime.Now;
                _stateManager.SaveOperation(newBoardShape);

                //reset the variables
                _lastDrawn = null;
            }

            return operations;
        }

        public override List<UXShape> Delete([NotNull] string shapeId)
        {
            // List of Operations to be send to UX
            List<UXShape> operations = new List<UXShape>();

            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);
            if (shapeFromManager == null)
            {
                Trace.WriteLine("Invalid Shape Id: Shape Id provided for Deletion of Shape does not exist in State Manager.");
                return new List<UXShape>();
            }

            UXShape oldShape = new UXShape(UXOperation.DELETE, shapeFromManager.MainShapeDefiner, shapeFromManager.Uid);
            operations.Add(oldShape);

            BoardShape shapeFromManagerClone = shapeFromManager.Clone();
            shapeFromManagerClone.RecentOperation = Operation.DELETE;
            shapeFromManagerClone.LastModifiedTime = DateTime.Now;

            _stateManager.SaveOperation(shapeFromManagerClone);

            _lastDrawn = null;
            return operations;

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
