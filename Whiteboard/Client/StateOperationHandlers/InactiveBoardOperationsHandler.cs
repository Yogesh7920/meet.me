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

namespace Whiteboard
{
    class InactiveBoardOperationsHandler : BoardOperationsState
    {

        public InactiveBoardOperationsHandler()
        {
            UserLevel = 0;
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

        public override List<UXShape> CreateShape(ShapeType shapetype, Coordinate start, Coordinate end,
                                                  float strokeWidth, BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation, Coordinate start,
                                                          Coordinate end, string shapeId, bool shapeComp = false)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Resize(Coordinate start, Coordinate end, string shapeId, DragPos dragPos)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Delete(string shapeId)
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Undo()
        {
            return new List<UXShape>();
        }

        public override List<UXShape> Redo()
        {
            return new List<UXShape>();
        }
    }
}
