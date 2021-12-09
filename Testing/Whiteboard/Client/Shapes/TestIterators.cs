/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 11/22/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Whiteboard;

namespace Testing.Whiteboard
{
    internal static class TestIterators
    {
        /// <summary>
        ///     Test cases for creation of shapes from previous shapes
        /// </summary>
        public static IEnumerable<TestCaseData> ShapeMaker_PreviousShape_ReturnsModifiedPreviousShape_TestCases
        {
            get
            {
                Coordinate stopDrag = new(7, 8);
                yield return new TestCaseData(4, 4, new Coordinate(5, 6), stopDrag).SetArgDisplayNames(
                    stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(7, 4);
                yield return new TestCaseData(0, 4, new Coordinate(5, 4), stopDrag).SetArgDisplayNames(
                    stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(1, 10);
                yield return new TestCaseData(6, 2, new Coordinate(2, 7), stopDrag).SetArgDisplayNames(
                    stopDrag.R.ToString(), stopDrag.C.ToString());
                stopDrag = new Coordinate(4, 5);
                yield return new TestCaseData(1, 1, new Coordinate((float) 3.5, (float) 4.5), stopDrag)
                    .SetArgDisplayNames(stopDrag.R.ToString(), stopDrag.C.ToString());
            }
        }

        /// <summary>
        ///     Test cases for creation of shapes from previous shapes
        /// </summary>
        public static IEnumerable<TestCaseData> ShapeMaker_PreviousLine_ReturnsModifiedPreviousShape_TestCases
        {
            get
            {
                Coordinate start = new(2, 2);
                Coordinate stopDrag = new(2, 4);
                var expectedWidth = GetDiff(stopDrag, start);
                var center = GetCenter(start, stopDrag);
                yield return new TestCaseData(expectedWidth, center, stopDrag, 0).SetArgDisplayNames("RightDrag");
                stopDrag = new Coordinate(4, 0);
                expectedWidth = GetDiff(stopDrag, start);
                center = GetCenter(start, stopDrag);
                yield return new TestCaseData(expectedWidth, center, stopDrag, (float) (3 * Math.PI / 4))
                    .SetArgDisplayNames("TopLeftDrag");
                stopDrag = new Coordinate(0, 0);
                expectedWidth = GetDiff(stopDrag, start);
                center = GetCenter(start, stopDrag);
                yield return new TestCaseData(expectedWidth, center, stopDrag, (float) (-3 * Math.PI / 4))
                    .SetArgDisplayNames("BottomLeft");
                stopDrag = new Coordinate(0, 4);
                expectedWidth = GetDiff(stopDrag, start);
                center = GetCenter(start, stopDrag);
                yield return new TestCaseData(expectedWidth, center, stopDrag, (float) (-Math.PI / 4))
                    .SetArgDisplayNames("BottomRight");
            }
        }

        /// <summary>
        ///     Test Cases for Rotation when originally the shape is in first quadrant.
        /// </summary>
        public static IEnumerable<TestCaseData> Rotate_Quad1_TestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(1, 1), (float) Math.PI / 4).SetArgDisplayNames("45_deg");
                yield return new TestCaseData(new Coordinate(1, 0), (float) Math.PI / 2).SetArgDisplayNames("90_deg");
                yield return new TestCaseData(new Coordinate(1, -1), (float) (3 * Math.PI) / 4).SetArgDisplayNames(
                    "135_deg");
                yield return new TestCaseData(new Coordinate(0, -1), (float) Math.PI).SetArgDisplayNames("180_deg");
                yield return new TestCaseData(new Coordinate(-1, -1), (float) (-3 * Math.PI) / 4).SetArgDisplayNames(
                    "225_deg");
                yield return new TestCaseData(new Coordinate(-1, 0), (float) -Math.PI / 2)
                    .SetArgDisplayNames("270_deg");
                yield return new TestCaseData(new Coordinate(-1, 1), (float) -Math.PI / 4)
                    .SetArgDisplayNames("315_deg");
                yield return new TestCaseData(new Coordinate(0, 1), 0).SetArgDisplayNames("360_deg");
            }
        }

        /// <summary>
        ///     Test Cases for Rotation when originally the shape is in second quadrant.
        /// </summary>
        public static IEnumerable<TestCaseData> Rotate_Quad2_TestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(1, 1), (float) (3 * Math.PI) / 4).SetArgDisplayNames(
                    "45_deg");
                yield return new TestCaseData(new Coordinate(1, 0), (float) Math.PI).SetArgDisplayNames("90_deg");
                yield return new TestCaseData(new Coordinate(1, -1), (float) (-3 * Math.PI) / 4).SetArgDisplayNames(
                    "135_deg");
                yield return new TestCaseData(new Coordinate(0, -1), (float) (-Math.PI / 2)).SetArgDisplayNames(
                    "180_deg");
                yield return new TestCaseData(new Coordinate(-1, -1), (float) -Math.PI / 4).SetArgDisplayNames(
                    "225_deg");
                yield return new TestCaseData(new Coordinate(-1, 0), 0).SetArgDisplayNames("270_deg");
                yield return new TestCaseData(new Coordinate(-1, 1), (float) Math.PI / 4).SetArgDisplayNames("315_deg");
                yield return new TestCaseData(new Coordinate(0, 1), (float) Math.PI / 2).SetArgDisplayNames("360_deg");
            }
        }

        /// <summary>
        ///     Test cases for resizing for all the four latches
        /// </summary>
        public static IEnumerable<TestCaseData> Resize_AllLatch_TestCases
        {
            get
            {
                float mag = 2;
                var cos60 = (float) Math.Cos(Math.PI / 3);
                var sin60 = (float) Math.Sin(Math.PI / 3);
                float deltaH = 2;
                var deltaW = 2 * (float) Math.Sqrt(3);
                var dragPos = DragPos.TopRight;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 + deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("TopRight_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 + deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("TopRight_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 - deltaH, 4 - deltaW,
                    dragPos).SetArgDisplayNames("TopRight_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 - deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("TopRight_Dec_H_Inc_W");


                dragPos = DragPos.TopLeft;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 + deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("TopLeft_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 + deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("TopLeft_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 - deltaH, 4 + deltaW,
                    dragPos).SetArgDisplayNames("TopLeft_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 - deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("TopLeft_Dec_H_Dec_W");

                dragPos = DragPos.BottomLeft;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 - deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("BottomRight_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 - deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("BottomRight_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 + deltaH, 4 + deltaW,
                    dragPos).SetArgDisplayNames("BottomRight_Inc_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 + deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("BottomRight_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(4 * sin60, 4 * cos60), BoardConstants.MinHeight,
                    BoardConstants.MinWidth, dragPos).SetArgDisplayNames("BottomLeft_MinH_MinW");

                dragPos = DragPos.BottomRight;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 - deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("BottomLeft_Dec_H_Inc_W");
                yield return new TestCaseData(new Coordinate(0, -mag), 4 - deltaH, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("BottomLeft_Dec_H_Dec_W");
                yield return new TestCaseData(new Coordinate(-mag * sin60, -mag * cos60), 4 + deltaH, 4 - deltaW,
                    dragPos).SetArgDisplayNames("BottomLeft_Inc_H_Dec_W");
                yield return new TestCaseData(new Coordinate(0, mag), 4 + deltaH, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("BottomLeft_Inc_H_Inc_W");

                dragPos = DragPos.Top;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 + deltaH, 4, dragPos)
                    .SetArgDisplayNames("Top_Inc_H");

                dragPos = DragPos.Bottom;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4 - deltaH, 4, dragPos)
                    .SetArgDisplayNames("Bottom_Dec_H");

                dragPos = DragPos.Right;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4, 4 + deltaW, dragPos)
                    .SetArgDisplayNames("Right_Inc_W");

                dragPos = DragPos.Left;
                yield return new TestCaseData(new Coordinate(mag * sin60, mag * cos60), 4, 4 - deltaW, dragPos)
                    .SetArgDisplayNames("Left_Dec_W");
            }
        }

        /// <summary>
        ///     Test cases for resizing for all the four latches
        /// </summary>
        public static IEnumerable<TestCaseData> Resize_AllLatchsforLine_TestCases
        {
            get
            {
                var w = (float) Math.Sqrt(32);
                float mag = 2;
                var cos60 = (float) Math.Cos(Math.PI / 3);
                var sin60 = (float) Math.Sin(Math.PI / 3);
                var dragPos = DragPos.TopRight;
                var changeW = 2 * (float) Math.Sqrt(3);
                yield return new TestCaseData(w + changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("TopRight_W_Inc_1");
                yield return new TestCaseData(w - changeW, new Coordinate(0, -mag), dragPos).SetArgDisplayNames(
                    "TopRight_W_Dec_1");
                yield return new TestCaseData(w - changeW, new Coordinate(-mag * sin60, -mag * cos60), dragPos)
                    .SetArgDisplayNames("TopRight_W_Dec_1");
                yield return new TestCaseData(w + changeW, new Coordinate(0, mag), dragPos).SetArgDisplayNames(
                    "TopRight_H_Inc_2");

                dragPos = DragPos.TopLeft;
                yield return new TestCaseData(w - changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("TopLeft_W_Dec_1");
                yield return new TestCaseData(w + changeW, new Coordinate(0, -mag), dragPos).SetArgDisplayNames(
                    "TopLeft_W_Inc_1");
                yield return new TestCaseData(w + changeW, new Coordinate(-mag * sin60, -mag * cos60), dragPos)
                    .SetArgDisplayNames("TopLeft_W_Inc_2");
                yield return new TestCaseData(w - changeW, new Coordinate(0, mag), dragPos).SetArgDisplayNames(
                    "TopLeft_W_Dec_2");

                dragPos = DragPos.BottomLeft;
                yield return new TestCaseData(w - changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("BottomLeft_W_Dec");

                dragPos = DragPos.BottomRight;
                yield return new TestCaseData(w + changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("BottomRight_W_Inc");

                dragPos = DragPos.Right;
                yield return new TestCaseData(w + changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("Right_W_Inc");

                dragPos = DragPos.Left;
                yield return new TestCaseData(w - changeW, new Coordinate(mag * sin60, mag * cos60), dragPos)
                    .SetArgDisplayNames("Left_W_Dec");
                yield return new TestCaseData(BoardConstants.MinWidth, new Coordinate(4 * sin60, 4 * cos60), dragPos)
                    .SetArgDisplayNames("Left_W_Dec");

                dragPos = DragPos.Top;
                yield return new TestCaseData(w, new Coordinate(mag * sin60, mag * cos60), dragPos).SetArgDisplayNames(
                    "Top_W_Same");
            }
        }

        /// <summary>
        ///     Finding Distance between two coordinates.
        /// </summary>
        /// <param name="a">Coordinate</param>
        /// <param name="b">Coordinate</param>
        /// <returns>Distance between coordinates.</returns>
        public static float GetDiff(Coordinate a, Coordinate b)
        {
            var delta = b - a;
            return (float) Math.Sqrt(Math.Pow(delta.R, 2) + Math.Pow(delta.C, 2));
        }

        /// <summary>
        ///     Midpoint between 2 points.
        /// </summary>
        /// <param name="a">Coordinate</param>
        /// <param name="b">Coordinate</param>
        /// <returns>Midpoint between coordinates.</returns>
        public static Coordinate GetCenter(Coordinate a, Coordinate b)
        {
            return (a + b) / 2;
        }
    }
}