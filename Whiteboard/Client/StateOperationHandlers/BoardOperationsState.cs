/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/01/2021
 * Date Modified: 11/01/2021
**/

using System.Collections.Generic;

namespace Whiteboard
{
    public abstract class BoardOperationsState
    {
        public abstract List<UXShape> ChangeHeight(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false);

        public abstract List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId);

        public abstract List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId);

        public abstract List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId);

        public abstract List<UXShape> ChangeWidth(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false);

        public abstract List<UXShape> CreateShape(ShapeType shapeType, Coordinate start, Coordinate end,
            float strokeWidth, BoardColor strokeColor, string shapeId = null, bool shapeComp = false);

        public abstract List<UXShape> Redo();

        public abstract List<UXShape> ResizeShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false);

        public abstract List<UXShape> RotateShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false);

        public abstract List<UXShape> TranslateShape(Coordinate start, Coordinate end, string shapeId,
            bool shapeComp = false);

        public abstract List<UXShape> Undo();
    }
}