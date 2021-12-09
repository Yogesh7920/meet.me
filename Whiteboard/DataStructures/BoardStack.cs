/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/26/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Whiteboard
{
    /// <summary>
    ///     Stack to store BoardShape before and just after the operation.
    /// </summary>
    public class BoardStack
    {
        /// <summary>
        ///     Capacity of the stack.
        /// </summary>
        private static int s_capacity;

        /// <summary>
        ///     The list which will act as stack.
        /// </summary>
        private readonly List<Tuple<BoardShape, BoardShape>> _stack;

        /// <summary>
        ///     Initializes BoardStack.
        /// </summary>
        /// <param name="capacity">Capacity of stack. Default is 7.</param>
        public BoardStack(int capacity = BoardConstants.UndoRedoStackSize)
        {
            s_capacity = capacity;
            _stack = new List<Tuple<BoardShape, BoardShape>>();
        }

        /// <summary>
        ///     Removes the first inserted element from the stack.
        /// </summary>
        private void RemoveFirstInserted()
        {
            if (GetSize() == BoardConstants.EmptySize) throw new InvalidOperationException("Stack is empty");
            _stack.RemoveAt(0);
        }

        /// <summary>
        ///     Removes the last inserted element from the stack.
        /// </summary>
        private void RemoveLastInserted()
        {
            if (GetSize() == BoardConstants.EmptySize) throw new InvalidOperationException("Stack is empty");
            _stack.RemoveAt(GetSize() - 1);
        }

        /// <summary>
        ///     Gets the size of the stack.
        /// </summary>
        /// <returns>Size of the stack</returns>
        public int GetSize()
        {
            return _stack.Count;
        }

        /// <summary>
        ///     Pushes the elements (BoardShape before and after the operation) on the top of the stack.
        /// </summary>
        /// <param name="boardShapePrevious">BoardShape before operation.</param>
        /// <param name="boardShapeNew">BoardShape after operation.</param>
        public void Push(BoardShape boardShapePrevious, BoardShape boardShapeNew)
        {
            if (GetSize() == s_capacity)
            {
                Trace.Indent();
                Trace.WriteLine(
                    "[Whiteboard] BoardStack.Push: Stack is at full capacity. Removing first inserted element.");
                Trace.Unindent();
                RemoveFirstInserted();
            }

            _stack.Add(Tuple.Create(boardShapePrevious, boardShapeNew));
        }

        /// <summary>
        ///     Finds the top element of the stack.
        /// </summary>
        /// <returns>Tuple of BoardShape before and after the operation.</returns>
        public Tuple<BoardShape, BoardShape> Top()
        {
            if (GetSize() == BoardConstants.EmptySize)
            {
                Trace.Indent();
                Trace.WriteLine("[Whiteboard] BoardStack.Top: Stack is empty.");
                Trace.Unindent();
                throw new IndexOutOfRangeException("Stack is empty");
            }

            return _stack[GetSize() - 1];
        }

        /// <summary>
        ///     Removes the top element from the stack.
        /// </summary>
        public void Pop()
        {
            if (GetSize() == BoardConstants.EmptySize)
            {
                Trace.Indent();
                Trace.WriteLine("[Whiteboard] BoardStack.Pop: Stack is empty.");
                Trace.Unindent();
                throw new IndexOutOfRangeException("Stack is empty");
            }

            RemoveLastInserted();
        }

        /// <summary>
        ///     Finds if the stack is empty or not.
        /// </summary>
        /// <returns>Boolean indicating if stack is empty.</returns>
        public bool IsEmpty()
        {
            return GetSize() == BoardConstants.EmptySize;
        }

        /// <summary>
        ///     Removes all elements from stack.
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
        }
    }
}