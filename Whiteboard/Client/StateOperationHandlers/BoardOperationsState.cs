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
    abstract public class BoardOperationsState
    {
        public int UserLevel { get; set; }

        abstract public List<UXShape> ChangeShapeFill(BoardColor shapeFill, string shapeId);

        abstract public List<UXShape> ChangeStrokeColor(BoardColor strokeColor, string shapeId);

        abstract public List<UXShape> ChangeStrokeWidth(float strokeWidth, string shapeId);

        abstract public List<UXShape> CreateShape(ShapeType shapeType, Coordinate start, Coordinate end,
                                                  float strokeWidth, BoardColor strokeColor, string shapeId = null,
                                                  bool shapeComp = false);

        abstract public List<UXShape> Redo();

        abstract public List<UXShape> ModifyShapeRealTime(RealTimeOperation realTimeOperation, Coordinate start, Coordinate end, string shapeId, bool shapeComp = false);

        abstract public List<UXShape> Delete(string shapeId);

        abstract public List<UXShape> Undo();
    }
}