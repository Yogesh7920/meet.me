/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 11/19/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using Whiteboard;

namespace Testing.Whiteboard
{
    /// <summary>
    ///     Static class to help test-classes.
    /// </summary>
    public static class StateManagerHelper
    {
        /// <summary>
        ///     Function to compare two board server shapes for equality
        /// </summary>
        /// <param name="shape1">First boardserver shape</param>
        /// <param name="shape2">Second boardserver shape</param>
        /// <returns>Returns true if equal else false.</returns>
        public static bool CompareBoardServerShapes(BoardServerShape shape1, BoardServerShape shape2)
        {
            return shape1.RequesterId == shape2.RequesterId && shape1.CheckpointNumber == shape2.CheckpointNumber
                                                            && shape1.OperationFlag == shape2.OperationFlag &&
                                                            shape1.CurrentCheckpointState ==
                                                            shape2.CurrentCheckpointState
                                                            && CompareBoardShapes(shape1.ShapeUpdates,
                                                                shape2.ShapeUpdates);
        }

        /// <summary>
        ///     Function to compare List of BoardShapes.
        /// </summary>
        /// <param name="shapes1">List of BoardShape</param>
        /// <param name="shapes2">List of BoardShape</param>
        /// <returns>True if equal else false</returns>
        public static bool CompareBoardShapes(List<BoardShape> shapes1, List<BoardShape> shapes2)
        {
            if (shapes1 == null && shapes2 == null) return true;
            if (shapes1.Count != shapes2.Count) return false;
            for (var i = 0; i < shapes1.Count; i++)
                if (!CompareBoardShapes(shapes1[i], shapes2[i]))
                    return false;
            return true;
        }

        /// <summary>
        ///     Compare two BoardShapes
        /// </summary>
        /// <param name="shape1">First BoardShape</param>
        /// <param name="shape2">Second BoardShape</param>
        /// <returns>True if equal else false</returns>
        public static bool CompareBoardShapes(BoardShape shape1, BoardShape shape2)
        {
            if (shape1 == null && shape2 == null) return true;
            return shape1.LastModifiedTime.Equals(shape2.LastModifiedTime) && shape1.Uid == shape2.Uid &&
                   shape1.CreationTime.Equals(shape2.CreationTime)
                   && shape1.RecentOperation == shape2.RecentOperation && shape1.ShapeOwnerId == shape2.ShapeOwnerId &&
                   shape1.UserLevel == shape2.UserLevel;
        }

        /// <summary>
        ///     Generate sorted boardshapes (sorted in order of timestamp)
        /// </summary>
        /// <param name="n">no. of shapes required.</param>
        /// <param name="operation">The operation for all the shapes. By default Modify</param>
        /// <returns>List of boardshapes</returns>
        public static List<BoardShape> GenerateSortedRandomBoardShapes(int n, Operation operation = Operation.Modify)
        {
            List<BoardShape> boardShapes = new();
            Random random = new();
            for (var i = 0; i < n; i++)
                boardShapes.Add(new BoardShape(null, random.Next(0, 2), DateTime.Now, DateTime.Now.AddMinutes(i),
                    RandomString(10), RandomString(5), operation));
            return boardShapes;
        }

        /// <summary>
        ///     Generate a boardshape with MainShape parameter
        /// </summary>
        /// <param name="operation">The operation for the shape</param>
        /// <returns>A boardShape</returns>
        public static BoardShape GetCompleteBoardShape(Operation operation)
        {
            Random random = new();
            return new BoardShape(new Rectangle(), random.Next(0, 2), DateTime.Now, DateTime.Now.AddMinutes(1),
                RandomString(10), RandomString(5), operation);
        }

        /// <summary>
        ///     Generate a sorted list of boardshapes with MainShape parameter
        /// </summary>
        /// <param name="n">The number of shapes in the list</param>
        /// <param name="operation">The operation on the shape</param>
        /// <returns>Sorted list of boardShapes.</returns>
        public static List<BoardShape> GetListCompleteBoardShapes(int n, Operation operation)
        {
            List<BoardShape> boardShapes = new();
            Random random = new();
            for (var i = 0; i < n; i++)
                boardShapes.Add(new BoardShape(new Rectangle(), random.Next(0, 2), DateTime.Now,
                    DateTime.Now.AddMinutes(i), RandomString(10), RandomString(5), operation));
            return boardShapes;
        }

        /// <summary>
        ///     Generates a random alpha-numeric string.
        /// </summary>
        /// <param name="length">Length of the string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Compares the order of UXShapes and BoardShapes by comparing UIDs.
        /// </summary>
        /// <param name="uXShapes">List of UXShape</param>
        /// <param name="boardShapes">List of BoardShapes</param>
        /// <returns>Boolean if order is same else false.</returns>
        public static bool CompareUXShapeOrder(List<UXShapeHelper> uXShapes, List<BoardShape> boardShapes)
        {
            if (uXShapes == null && boardShapes == null) return true;
            if (uXShapes == null || boardShapes == null) return false;
            if (uXShapes.Count != boardShapes.Count) return false;
            for (var i = 0; i < uXShapes.Count; i++)
                if (uXShapes[i].ShapeId != boardShapes[i].Uid)
                    return false;
            return true;
        }
    }
}