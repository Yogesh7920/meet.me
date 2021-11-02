/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/13/2021
 * Date Modified: 10/13/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard.Client
{
    class WhiteBoardOperationHandler : IWhiteBoardOperationHandler
    {
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
        public List<UXShape> ChangeShapeFill(Color shapeFill, string shapeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the Stroke Color of the shape outline.
        /// </summary>
        /// <param name="strokeColor"> Stroke Color. </param>
        /// <param name="shapeId">Id of the shape. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeStrokeColor(Color strokeColor, string shapeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the thickness of the shape outline stroke.
        /// </summary>
        /// <param name="strokeWidth"> Stroke Thickness. </param>
        /// <param name="shapeId">Id of the shape. </param>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            throw new NotImplementedException();
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
        public List<UXShape> CreateEllipse(Coordinate start, Coordinate end, float strokeWidth, Color strokeColor, string shapeId = null, bool shapeComp = false)
        {
            throw new NotImplementedException();
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
        public List<UXShape> CreateLine(Coordinate start, Coordinate end, float strokeWidth, Color strokeColor, string shapeId = null, bool shapeComp = false)
        {
            throw new NotImplementedException();
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
        public List<UXShape> CreatePolyline(Coordinate start, Coordinate end, float strokeWidth, Color strokeColor, string shapeId = null, bool shapeComp = false)
        {
            throw new NotImplementedException();
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
        public List<UXShape> CreateRectangle(Coordinate start, Coordinate end, float strokeWidth, Color strokeColor, string shapeId = null, bool shapeComp = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets owner of the shape with a shape Id.
        /// </summary>
        /// <param name="shapeId"> Id of the shape. </param>
        /// <returns> User Name. </returns>
        public string getUserName(string shapeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs Redo.
        /// </summary>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> Redo()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Switches WhiteBoard state from active to inactive and vice-versa.
        /// </summary>
        /// <returns> Denotes succesfull state switch. </returns>
        public bool switchState()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs Undo.
        /// </summary>
        /// <returns> List of UXShapes for UX to render. </returns>
        public List<UXShape> Undo()
        {
            throw new NotImplementedException();
        }
    }
}
