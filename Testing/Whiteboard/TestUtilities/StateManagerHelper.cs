using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whiteboard;

namespace Testing.Whiteboard
{
    public static class StateManagerHelper
    {
        public static bool CompareBoardServerShapes(BoardServerShape shape1, BoardServerShape shape2)
        {
            return (shape1.RequesterId == shape2.RequesterId) && (shape1.CheckpointNumber == shape2.CheckpointNumber)
                && (shape1.OperationFlag == shape2.OperationFlag) && (shape1.CurrentCheckpointState == shape2.CurrentCheckpointState)
                && (CompareBoardShapes(shape1.ShapeUpdates, shape2.ShapeUpdates));
        }

        public static bool CompareBoardShapes(List<BoardShape> shapes1, List<BoardShape> shapes2)
        {
            if (shapes1 == null && shapes2 == null)
            {
                return true;
            }
            if (shapes1.Count != shapes2.Count)
            {
                return false;
            }
            for (int i = 0; i < shapes1.Count; i++)
            {
                if (!CompareBoardShapes(shapes1[i], shapes2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CompareBoardShapes(BoardShape shape1, BoardShape shape2)
        {
            if(shape1==null && shape2 == null)
            {
                return true;
            }
            return shape1.LastModifiedTime.Equals(shape2.LastModifiedTime) && (shape1.Uid == shape2.Uid) && shape1.CreationTime.Equals(shape2.CreationTime)
                && (shape1.RecentOperation == shape2.RecentOperation) && (shape1.ShapeOwnerId == shape2.ShapeOwnerId) && (shape1.UserLevel == shape2.UserLevel);
  
        }

        public static List<BoardShape> GenerateSortedRandomBoardShapes(int n, Operation operation = Operation.MODIFY)
        {
            List<BoardShape> boardShapes = new();
            Random random = new();
            for (int i = 0; i < n; i++)
            {
                boardShapes.Add(new(null, random.Next(0, 2), DateTime.Now, DateTime.Now.AddMinutes(i), RandomString(10), RandomString(5), operation));
            }
            return boardShapes;
        }

        public static BoardShape GetCompleteBoardShape(Operation operation)
        {
            Random random = new();
            return new BoardShape(new Rectangle(), random.Next(0, 2), DateTime.Now, DateTime.Now.AddMinutes(1), RandomString(10), RandomString(5), operation);
        }

        public static List<BoardShape> GetListCompleteBoardShapes(int n, Operation operation)
        {
            List<BoardShape> boardShapes = new();
            Random random = new();
            for(int i = 0; i < n; i++)
            {
                boardShapes.Add(new BoardShape(new Rectangle(), random.Next(0, 2), DateTime.Now, DateTime.Now.AddMinutes(i), RandomString(10), RandomString(5), operation));
            }
            return boardShapes;
        }

        public static string RandomString(int length)
        {
            Random random = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool CompareUXShapeOrder(List<UXShape> uXShapes, List<BoardShape> boardShapes)
        {
            if(uXShapes == null && boardShapes == null)
            {
                return true;
            }
            if(uXShapes == null || boardShapes == null)
            {
                return false;
            }
            if(uXShapes.Count != boardShapes.Count)
            {
                return false;
            }
            for(int i = 0; i < uXShapes.Count; i++)
            {
                if(uXShapes[i].WindowsShape.Uid != boardShapes[i].Uid)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
