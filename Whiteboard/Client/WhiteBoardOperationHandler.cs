/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 11/02/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public class WhiteBoardOperationHandler : IWhiteBoardOperationHandler
    {

        private Coordinate _canvasSize;
        private BoardOperationsState _boardState;
        private ActiveBoardOperationsHandler _activeBoardOperationsHandler;
        private InactiveBoardOperationsHandler _inactiveBoardOperationsHandler;
        private BoardState _boardStateIdentifier;

        public WhiteBoardOperationHandler(Coordinate canvasSize)
        {
            _canvasSize = canvasSize;
            _activeBoardOperationsHandler = new ActiveBoardOperationsHandler(_canvasSize);
            _inactiveBoardOperationsHandler = new InactiveBoardOperationsHandler(_canvasSize);
            _boardState = _activeBoardOperationsHandler;
            _boardStateIdentifier = BoardState.ACTIVE;
        }

        /// <summary>
        /// Changes the height of shape with given shape ID.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <param name="shapeComp"> Indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeHeight(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the Fill Color of the shape.
        /// </summary>
        /// <param name="shapeFill"> Shape Fill Color. </param>
        /// <param name="shapeId">Id of the shape. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            return _boardState.ChangeShapeFill(shapeFill, shapeId);
        }

        /// <summary>
        /// Changes the Stroke Color of the shape outline.
        /// </summary>
        /// <param name="strokeColor"> Stroke Color. </param>
        /// <param name="shapeId">Id of the shape. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            return _boardState.ChangeStrokeColor(strokeColor, shapeId);
        }

        /// <summary>
        /// Changes the thickness of the shape outline stroke.
        /// </summary>
        /// <param name="strokeWidth"> Stroke Thickness. </param>
        /// <param name="shapeId">Id of the shape. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            return _boardState.ChangeStrokeWidth(strokeWidth, shapeId);
        }

        /// <summary>
        /// Changes the width of the shape with given shape ID.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <param name="shapeComp"> Indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeWidth(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates Ellipse/Circle.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="strokeWidth"> Shape boundary stroke width. </param>
        /// <param name="strokeColor"> Color of shape boundary stroke. </param>
        /// <param name="shapeId"> Id of the shape. Null if shape creation just started. </param>
        /// <param name="shapeComp"> indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> CreateEllipse(Coordinate start, Coordinate end, float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false)
        {
            return _boardState.CreateShape(ShapeType.ELLIPSE, start, end, strokeWidth, strokeColor, shapeId, shapeComp);
        }

        /// <summary>
        /// Creates straight line.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="strokeWidth"> Shape boundary stroke width. </param>
        /// <param name="strokeColor"> Color of shape boundary stroke. </param>
        /// <param name="shapeId"> Id of the shape. Null if shape creation just started. </param>
        /// <param name="shapeComp"> indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> CreateLine(Coordinate start, Coordinate end, float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false)
        {
            return _boardState.CreateShape(ShapeType.LINE, start, end, strokeWidth, strokeColor, shapeId, shapeComp);
        }

        /// <summary>
        /// Creates Polyline.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="strokeWidth"> Shape boundary stroke width. </param>
        /// <param name="strokeColor"> Color of shape boundary stroke. </param>
        /// <param name="shapeId"> Id of the shape. Null if shape creation just started. </param>
        /// <param name="shapeComp"> indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> CreatePolyline(Coordinate start, Coordinate end, float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false)
        {
            return _boardState.CreateShape(ShapeType.POLYLINE, start, end, strokeWidth, strokeColor, shapeId, shapeComp);
        }

        /// <summary>
        /// Creates Rectangle/Square.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="strokeWidth"> Shape boundary stroke width. </param>
        /// <param name="strokeColor"> Color of shape boundary stroke. </param>
        /// <param name="shapeId"> Id of the shape. Null if shape creation just started. </param>
        /// <param name="shapeComp"> indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> CreateRectangle(Coordinate start, Coordinate end, float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false)
        {
            return _boardState.CreateShape(ShapeType.RECTANGLE, start, end, strokeWidth, strokeColor, shapeId, shapeComp);
        }

        /// <summary>
        /// Delete the shape with given shape ID.
        /// </summary>
        /// <param name="shapeId">Id of the shape.</param>
        /// <returns></returns>
        public List<UXShape> DeleteShape(string shapeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets owner of the shape with a shape Id.
        /// </summary>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <returns> User Name. </returns>
        public string GetUserName(string shapeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs Redo.
        /// </summary>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> Redo()
        {
            return _boardState.Redo();
        }

        /// <summary>
        /// Resizes the shape with given shape ID.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <param name="shapeComp"> Indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ResizeShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            return _boardState.ModifyShapeRealTime(RealTimeOperation.RESIZE, start, end, shapeId, shapeComp);
        }

        /// <summary>
        /// Rotates the shape with given shape ID.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <param name="shapeComp"> Indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> RotateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            return _boardState.ModifyShapeRealTime(RealTimeOperation.ROTATE, start, end, shapeId, shapeComp);
        }

        /// <summary>
        /// Switches WhiteBoard state from active to inactive and vice-versa.
        /// </summary>
        /// <returns> Denotes succesfull state switch. </returns>
        public bool SwitchState()
        {
            if (_boardStateIdentifier == BoardState.ACTIVE)
            {
                _boardState = _inactiveBoardOperationsHandler;
                _boardStateIdentifier = BoardState.INACTIVE;
            }
            else
            {
                _boardState = _activeBoardOperationsHandler;
                _boardStateIdentifier = BoardState.ACTIVE;
            }
            return true;
        }

        /// <summary>
        /// Translates the shape with given shape ID.
        /// </summary>
        /// <param name="start"> Coordinate of mouse down event. </param>
        /// <param name="end"> Current cordinate to display real-time shape creation before/at mouse up event. </param>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <param name="shapeComp"> Indicative of a mouse up event. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> TranslateShape(Coordinate start, Coordinate end, string shapeId, bool shapeComp = false)
        {
            return _boardState.ModifyShapeRealTime(RealTimeOperation.TRANSLATE, start, end, shapeId, shapeComp);
        }

        /// <summary>
        /// Performs Undo.
        /// </summary>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> Undo()
        {
            return _boardState.Redo();
        }

        public void SetUserLevel(int userlevel)
        {
            _boardState.UserLevel = userlevel;
        }
    }
}
