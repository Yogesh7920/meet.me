/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/01/2021
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
        private int UserLevel;

        public ActiveBoardOperationsHandler()
        {
            _isLastDrawPending = false;
            _lastDrawn = null;
            _stateManager = ClientBoardStateManager.Instance;
        }
        
        public override List<UXShape> ChangeHeight(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        public override List<UXShape> ChangeWidth(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }
        //public delete

        public override List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape ShapeFromManager = _stateManager.GetBoardShape(shapeId);

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape NewBoardShape = ShapeFromManager.Clone();
            NewBoardShape.MainShapeDefiner.ShapeFill = shapeFill.Clone();

            List<UXShape> Operations = UpdateManager(ShapeFromManager, NewBoardShape, Operation.MODIFY);

            return Operations;
        }

        public override List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape ShapeFromManager = _stateManager.GetBoardShape(shapeId);

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape NewBoardShape = ShapeFromManager.Clone();
            NewBoardShape.MainShapeDefiner.StrokeColor = strokeColor.Clone();

            List<UXShape> Operations = UpdateManager(ShapeFromManager, NewBoardShape, Operation.MODIFY);

            return Operations;
        }

        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            // get the actual BoardServer object stored in the server
            BoardShape ShapeFromManager = _stateManager.GetBoardShape(shapeId);

            // Create a new BoardShape to perform modification since this changes should not be directly reflected in the object stored in the Manager.
            BoardShape NewBoardShape = ShapeFromManager.Clone();
            NewBoardShape.MainShapeDefiner.StrokeWidth = strokeWidth;

            List<UXShape> Operations = UpdateManager(ShapeFromManager, NewBoardShape, Operation.MODIFY);

            return Operations;
        }

        private List<UXShape> UpdateManager(BoardShape oldBoardShape, BoardShape newBoardShape, Operation operationType)
        {
            // Whenever Update goes to the Manager, the last operation being performed in discarded.
            // Set prev drawings to null in case any exists.
            _lastDrawn = null;
            _isLastDrawPending = false;

            List<UXShape> Operations = new List<UXShape>();

            string OldShapeId = oldBoardShape.ShapeOwnerId;

            // Creation of UXShapes and appending them to put the list containing all operations.
            UXShape OldShape = new UXShape(UXOperation.DELETE, oldBoardShape.MainShapeDefiner, OldShapeId);
            Operations.Add(OldShape);

            UXShape UxNewShape = new UXShape(UXOperation.CREATE, newBoardShape.MainShapeDefiner, OldShapeId);

            Operations.Add(UxNewShape);

            newBoardShape.RecentOperation = operationType;
            newBoardShape.LastModifiedTime = DateTime.Now;

            _stateManager.SaveOperation(newBoardShape);

            return Operations;
        }

        public override List<UXShape> CreateShape(ShapeType shapeType, Coordinate start, Coordinate end,
                                                  float strokeWidth, BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false)
        {

            // List of Operations to be send to UX
            List<UXShape> Operations = new List<UXShape>();
             
            if (_isLastDrawPending == false)
            {
                _lastDrawn = null;
            }
            // required to clear the previous shape in internal storage for real time rendering, before forming the new shape
            else if ((_lastDrawn != null) && (shapeType != _lastDrawn.MainShapeDefiner.ShapeIdentifier))
            {
                // Delete the Object that was already created in the canvas
                string oldShapeId = _lastDrawn.Uid;
                UXShape OldShape = new UXShape(UXOperation.DELETE, _lastDrawn.MainShapeDefiner, oldShapeId);
                Operations.Add(OldShape);

                // In this case previous shape won't be used for creating new shape
                _lastDrawn = null;
            }

            //creation of a completely new shape
            string NewShapeId = null;
            if (_lastDrawn == null)
            {
                MainShape newMainShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, null);

                // setting params for new shape
                newMainShape.StrokeColor = strokeColor.Clone();
                newMainShape.StrokeWidth = strokeWidth;

                UXShape newUxShape = new UXShape(UXOperation.CREATE, newMainShape, null);
                NewShapeId = newUxShape.WindowsShape.Uid;
                Operations.Add(newUxShape);

                int UserLevel = 0; // to be changed
                string userId = _stateManager.GetUser();

                // creating the new BoardShape to send to server
                _lastDrawn = new BoardShape(newMainShape, UserLevel, DateTime.Now, DateTime.Now, NewShapeId, userId, Operation.CREATE);
            }
            else
            {
                MainShape modifiedPrevShape = ShapeFactory.MainShapeCreatorFactory(shapeType, start, end, _lastDrawn.MainShapeDefiner);
                UXShape newUxShape = new UXShape(UXOperation.CREATE, modifiedPrevShape, null);
                NewShapeId = newUxShape.WindowsShape.Uid;
                Operations.Add(newUxShape);

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
                newBoardShape.Uid = NewShapeId;
                newBoardShape.LastModifiedTime = DateTime.Now;
                newBoardShape.CreationTime = DateTime.Now;
                _stateManager.SaveOperation(newBoardShape);
               
                //reset the variables
                _lastDrawn = null;
                _isLastDrawPending = false;
            }

            return Operations;
        }

        public override List<UXShape> Redo()
        {
            throw new NotImplementedException();
        }

        public override List<UXShape> ResizeShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        public override List<UXShape> RotateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        public override List<UXShape> TranslateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        public override List<UXShape> Undo()
        {
            throw new NotImplementedException();
        }
    }
}
