/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/01/2021
**/

using System.Collections.Generic;

namespace Whiteboard
{
    internal class InactiveBoardOperationsHandler : BoardOperationsState
    {
        public override List<UXShape> ChangeHeight(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ChangeWidth(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> CreateShape(ShapeType shapetype, Coordinate start, Coordinate end,
            float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Redo()
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ResizeShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> RotateShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> TranslateShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Undo()
        {
            return new List<UXShape>();
        }
    }
}