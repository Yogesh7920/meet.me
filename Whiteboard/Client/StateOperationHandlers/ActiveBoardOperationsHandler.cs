/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/02/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace Whiteboard
{
    class ActiveBoardOperationsHandler : BoardOperationsState
    {

        private BoardShape _lastDrawn;
        private bool _isLastDrawPending;
        private IClientBoardStateManagerInternal _stateManager;
        private int _userLevel;
        private Coordinate _canvasSize;

        public ActiveBoardOperationsHandler(Coordinate canvasSize)
        {
            _isLastDrawPending = false;
            _lastDrawn = null;
            _stateManager = ClientBoardStateManager.Instance;
            _canvasSize = canvasSize;
        }
        
        /// <summary>
        /// Change the Height of the shape.
        /// </summary>
        /// <param name="start">Start Coordinate of mouse drag.</param>
        /// <param name="end">End Coordinate of mouse drag.</param>
        /// <param name="shapeId">The ShapeId to perform the operation.</param>
        /// <param name="shapeComp">Determines whether shape creation is complete and can be sent to manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeHeight(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the Width of the shape.
        /// </summary>
        /// <param name="start">Start Coordinate of mouse drag.</param>
        /// <param name="end">End Coordinate of mouse drag.</param>
        /// <param name="shapeId">The ShapeId to perform the operation.</param>
        /// <param name="shapeComp">Determines whether shape creation is complete and can be sent to manager.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeWidth(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }
        //public delete - to implemented

        /// <summary>
        /// Changes the shape fill of the shape.
        /// </summary>
        /// <param name="shapeFill">Modified fill color of the shape..</param>
        /// <param name="shapeId">Id of the shape on which operation is performed.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);

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
        public override List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);

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
        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape shapeFromManager = _stateManager.GetBoardShape(shapeId);

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
        private List<UXShape> UpdateManager(BoardShape oldBoardShape, BoardShape newBoardShape, Operation operationType)
        {
            // Whenever Update goes to the Manager, the last operation saved locally is discarded.
            // Set prev drawings to null in case any exists.
            _lastDrawn = null;
            _isLastDrawPending = false;

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
        public override List<UXShape> CreateShape(ShapeType shapeType, Coordinate start, Coordinate end,
                                                  float strokeWidth, BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false)
        {
            // shapeId null signifies a new shape creation.
            if(shapeId == null)
            {
                _lastDrawn = null;
                _isLastDrawPending = false;
            }

            // List of Operations to be send to UX
            List<UXShape> operations = new List<UXShape>();
 
            // required to clear the previous shape in internal storage for real time rendering, before forming the new shape
            if ((_lastDrawn != null) && (shapeType != _lastDrawn.MainShapeDefiner.ShapeIdentifier))
            {
                // Delete the Object that was already created in the canvas
                string oldShapeId = _lastDrawn.Uid;
                UXShape oldShape = new UXShape(UXOperation.DELETE, _lastDrawn.MainShapeDefiner, oldShapeId);
                operations.Add(oldShape);

                // In this case previous shape won't be used for creating new shape
                _lastDrawn = null;
            }

            //creation of a completely new shape
            string newShapeId = null;
            if (_lastDrawn == null)
            {
                MainShape newMainShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);

                // setting params for new shape
                newMainShape.StrokeColor = strokeColor.Clone();
                newMainShape.StrokeWidth = strokeWidth;

                UXShape newUxShape = new UXShape(UXOperation.CREATE, newMainShape, null);
                newShapeId = newUxShape.WindowsShape.Uid;
                operations.Add(newUxShape);

                int userLevel = 0; // to be changed
                string userId = _stateManager.GetUser();

                // creating the new BoardShape to send to server
                _lastDrawn = new BoardShape(newMainShape, userLevel, DateTime.Now, DateTime.Now, newShapeId, userId, Operation.CREATE);
            }
            else
            {
                MainShape modifiedPrevShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, _lastDrawn.MainShapeDefiner);
                UXShape newUxShape = new UXShape(UXOperation.CREATE, modifiedPrevShape, null);
                newShapeId = newUxShape.WindowsShape.Uid;
                operations.Add(newUxShape);

                _lastDrawn.MainShapeDefiner = modifiedPrevShape;
                _lastDrawn.LastModifiedTime = DateTime.Now;
                _lastDrawn.RecentOperation = Operation.CREATE;
            }
            
            // sending the Updates to the Client Side Server
            _isLastDrawPending = true;
            if (shapeComp)
            {
                // send updates to server .. clone of _lastDrawn
                BoardShape newBoardShape = _lastDrawn.Clone();
                newBoardShape.RecentOperation = Operation.CREATE;
                newBoardShape.Uid = newShapeId;
                newBoardShape.LastModifiedTime = DateTime.Now;
                newBoardShape.CreationTime = DateTime.Now;
                _stateManager.SaveOperation(newBoardShape);
               
                //reset the variables
                _lastDrawn = null;
                _isLastDrawPending = false;
            }

            return operations;
        }

        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Redo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resize the shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse Drag.</param>
        /// <param name="shapeId">Id of the shape translated.</param>
        /// <param name="shapeComp">Denotes whether the shape is complete, or if it is for real-time rendering.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> ResizeShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rotate the shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse Drag.</param>
        /// <param name="shapeId">Id of the shape translated.</param>
        /// <param name="shapeComp">Denotes whether the shape is complete, or if it is for real-time rendering.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> RotateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate on shape.
        /// </summary>
        /// <param name="start">Start of mouse drag.</param>
        /// <param name="end">End of mouse Drag.</param>
        /// <param name="shapeId">Id of the shape translated.</param>
        /// <param name="shapeComp">Denotes whether the shape is complete, or if it is for real-time rendering.</param>
        /// <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> TranslateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform Undo Operation.
        /// </summary>
        ///  <returns>The List of operations on Shapes for UX to render.</returns>
        public override List<UXShape> Undo()
        {
            throw new NotImplementedException();
        }
    }
}
