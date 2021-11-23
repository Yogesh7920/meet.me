/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/23/2021
**/

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard;

namespace Testing.Whiteboard
{
    static class TestIterators
    {
        /// <summary>
        /// Test cases for creation of shapes from previous shapes
        /// </summary>
        public static IEnumerable<TestCaseData> ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape_TestCases
        {
            get
            {
                Coordinate stopDrag = new(7, 8);
                yield return new TestCaseData(4, 4, new Coordinate(5, 6), stopDrag).SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(7, 4);
                yield return new TestCaseData(0, 4, new Coordinate(5, 4), stopDrag).SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(1, 10);
                yield return new TestCaseData(6, 2, new Coordinate(2, 7), stopDrag).SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(7, 8);
                yield return new TestCaseData(4, 4, new Coordinate(5, 6), stopDrag).SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(4, 5);
                yield return new TestCaseData(1, 1, new Coordinate((float)3.5, (float)4.5), stopDrag).SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
            }
        }

        /// <summary>
        /// Test Cases for Rotation when originally the shape is in first quadrant.
        /// </summary>
        public static IEnumerable<TestCaseData> Rotate_Quad1_TestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(1, 1), (float)Math.PI / 4).SetArgDisplayNames("45_deg");
                yield return new TestCaseData(new Coordinate(1, 0), (float)Math.PI / 2).SetArgDisplayNames("90_deg");
                yield return new TestCaseData(new Coordinate(1, -1), (float)(3 * Math.PI) / 4).SetArgDisplayNames("135_deg");
                yield return new TestCaseData(new Coordinate(0, -1), (float)Math.PI).SetArgDisplayNames("180_deg");
                yield return new TestCaseData(new Coordinate(-1, -1), (float)(-3 * Math.PI) / 4).SetArgDisplayNames("225_deg");
                yield return new TestCaseData(new Coordinate(-1, 0), (float)-Math.PI / 2).SetArgDisplayNames("270_deg");
                yield return new TestCaseData(new Coordinate(-1, 1), (float)(-Math.PI) / 4).SetArgDisplayNames("315_deg");
                yield return new TestCaseData(new Coordinate(0, 1), 0).SetArgDisplayNames("360_deg");
            }
        }

        /// <summary>
        ///  Test Cases for Rotation when originally the shape is in second quadrant.
        /// </summary>
        public static IEnumerable<TestCaseData> Rotate_Quad2_TestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(1, 1), (float)(3 * Math.PI) / 4).SetArgDisplayNames("45_deg");
                yield return new TestCaseData(new Coordinate(1, 0), (float)Math.PI).SetArgDisplayNames("90_deg");
                yield return new TestCaseData(new Coordinate(1, -1), (float)(-3 * Math.PI) / 4).SetArgDisplayNames("135_deg");
                yield return new TestCaseData(new Coordinate(0, -1), (float)(-Math.PI / 2)).SetArgDisplayNames("180_deg");
                yield return new TestCaseData(new Coordinate(-1, -1), (float)(-Math.PI) / 4).SetArgDisplayNames("225_deg");
                yield return new TestCaseData(new Coordinate(-1, 0), 0).SetArgDisplayNames("270_deg");
                yield return new TestCaseData(new Coordinate(-1, 1), (float)(Math.PI) / 4).SetArgDisplayNames("315_deg");
                yield return new TestCaseData(new Coordinate(0, 1), (float)(Math.PI) / 2).SetArgDisplayNames("360_deg");
            }
        }

        /// <summary>
        /// Test cases for resizing for all the four latches
        /// </summary>
        public static IEnumerable<TestCaseData> Resize_AllLatch_TestCases
        {
            get
            {
                float mag = 2;
                float cos60 = (float)Math.Cos(Math.PI / 3);
                float sin60 = (float)Math.Sin(Math.PI / 3);
                float deltaH = 2;
                float deltaW = 2 * (float)Math.Sqrt(3);
                DragPos dragPos = DragPos.TOP_RIGHT;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 + deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("TopRight_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 + deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("TopRight_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 - deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("TopRight_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 - deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("TopRight_Dec_H_Inc_W");

                dragPos = DragPos.TOP_LEFT;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 + deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("TopLeft_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 + deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("TopLeft_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 - deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("TopLeft_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 - deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("TopLeft_Dec_H_Dec_W");

                dragPos = DragPos.BOTTOM_LEFT;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 - deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("BottomRight_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 - deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("BottomRight_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 + deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("BottomRight_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 + deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("BottomRight_Inc_H_Dec_W");

                dragPos = DragPos.BOTTOM_RIGHT;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 - deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("BottomLeft_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 - deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("BottomLeft_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 + deltaH, 4 - deltaW, dragPos).SetArgDisplayNames("BottomLeft_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 + deltaH, 4 + deltaW, dragPos).SetArgDisplayNames("BottomLeft_Inc_H_Inc_W");
            }
        }
    }
}
