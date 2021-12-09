/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/25/2021
 * Date Modified: 11/28/2021
**/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Whiteboard
{
    /// <summary>
    ///     Priority queue for QueueElement, with priority based on Timestamp.
    /// </summary>
    public class BoardPriorityQueue
    {
        /// <summary>
        ///     the array to store elements of priority queue.
        /// </summary>
        private readonly List<QueueElement> _queue;

        /// <summary>
        ///     Initializes queue. (Constructor)
        /// </summary>
        public BoardPriorityQueue()
        {
            Trace.WriteLine("[Whiteboard] BoardPriorityQueue: Initializing priority queue");
            _queue = new List<QueueElement>();
        }

        /// <summary>
        ///     Gets the size of the queue.
        /// </summary>
        /// <returns>Size of the queue.</returns>
        public int GetSize()
        {
            return _queue.Count;
        }

        /// <summary>
        ///     Finds the index of parent.
        /// </summary>
        /// <param name="childIndex">Child's index whose parent needs to be found.</param>
        /// <returns>Index of parent.</returns>
        private static int Parent(int childIndex)
        {
            return (childIndex - 1) / 2;
        }

        /// <summary>
        ///     Finds the index of right child.
        /// </summary>
        /// <param name="parentIndex">Parent's index whose right child needs to be found.</param>
        /// <returns>Index of right child.</returns>
        private static int RightChild(int parentIndex)
        {
            return 2 * parentIndex + 2;
        }

        /// <summary>
        ///     Finds the index of left child.
        /// </summary>
        /// <param name="parentIndex">Parent's index whose left child needs to be found.</param>
        /// <returns>Index of left child.</returns>
        private static int LeftChild(int parentIndex)
        {
            return 2 * parentIndex + 1;
        }

        /// <summary>
        ///     Swaps two elements of the queue.
        /// </summary>
        /// <param name="index1">Index of first element.</param>
        /// <param name="index2">Index of second element.</param>
        private void SwapElements(int index1, int index2)
        {
            // checking validity of indexes
            if (index1 >= GetSize() || index1 < BoardConstants.EmptySize || index2 >= GetSize() ||
                index2 < BoardConstants.EmptySize)
                throw new IndexOutOfRangeException("Index value out of range. Swapping can't happen.");

            // interchanging index values of the element
            _queue[index1].Index = index2;
            _queue[index2].Index = index1;

            // swapping elements
            var queueElement = _queue[index1];
            _queue[index1] = _queue[index2];
            _queue[index2] = queueElement;
        }

        /// <summary>
        ///     Max heapify the subtree with root at given index recursively.
        ///     Assumes that the subtrees are already max-heapified.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        private void Heapify(int index)
        {
            // checking validity of indexes
            if (index < BoardConstants.EmptySize || index >= GetSize())
                throw new IndexOutOfRangeException("Index value out of range. Can't Heapify.");

            var leftChild = LeftChild(index);
            var rightChild = RightChild(index);
            var largest = index;

            // find the latest between parent and its children
            if (leftChild < GetSize() && _queue[leftChild].Timestamp > _queue[index].Timestamp) largest = leftChild;
            if (rightChild < GetSize() && _queue[rightChild].Timestamp > _queue[largest].Timestamp)
                largest = rightChild;

            // if a child is later than the parent, swap parent with the latest child and call Heapify on subtree below. 
            if (largest != index)
            {
                SwapElements(index, largest);
                Heapify(largest);
            }
        }

        /// <summary>
        ///     Gets the top-most element of the priority queue.
        /// </summary>
        /// <returns>QueueElement having highest priority or null if queue is empty.</returns>
        public QueueElement Top()
        {
            if (GetSize() == BoardConstants.EmptySize)
            {
                Trace.Indent();
                Trace.WriteLine("[Whiteboard] BoardPriorityQueue.Top: Priority queue empty, returning null");
                Trace.Unindent();
                return null;
            }

            return _queue[0];
        }

        /// <summary>
        ///     Inserts the element in the priority queue.
        /// </summary>
        /// <param name="queueElement">Element to be inserted.</param>
        public void Insert(QueueElement queueElement)
        {
            Trace.Indent();
            // update the index of the queue element
            var lastIndex = GetSize();
            queueElement.Index = lastIndex;

            // adding the element in queue and placing it at right position. 
            _queue.Add(queueElement);
            while (lastIndex != 0 && _queue[Parent(lastIndex)].Timestamp < _queue[lastIndex].Timestamp)
            {
                SwapElements(lastIndex, Parent(lastIndex));
                lastIndex = Parent(lastIndex);
            }

            Trace.WriteLine("[Whiteboard] BoardPriorityQueue.Insert: Inserted the element successfully.");
            Trace.Unindent();
        }

        public void Insert(List<QueueElement> queueElements)
        {
            Trace.Indent();
            for (var i = 0; i < queueElements.Count; i++) Insert(queueElements[i]);
            Trace.WriteLine("[Whiteboard] BoardPriorityQueue.Insert: Inserted the list of elements successfully.");
            Trace.Unindent();
        }

        /// <summary>
        ///     Removes the latest(in terms of timestamp) element.
        /// </summary>
        /// <returns>The latest element or null if priority queue is empty.</returns>
        public QueueElement Extract()
        {
            Trace.Indent();
            if (GetSize() == BoardConstants.EmptySize)
            {
                Trace.WriteLine("[Whiteboard] BoardPriorityQueue.Extract: No element present, returning null.");
                Trace.Unindent();
                return null;
            }

            if (GetSize() == 1)
            {
                var queueElement = _queue[0];

                // removing the last and only element from the queue.
                _queue.RemoveAt(0);

                Trace.WriteLine(
                    "[Whiteboard] BoardPriorityQueue.Extract: One element present, the priority queue is empty now.");
                Trace.Unindent();
                return queueElement;
            }
            else
            {
                var queueElement = _queue[0];

                // putting the last element at root, then remove it from the queue and call Heapify on the root. 
                _queue[0] = _queue[GetSize() - 1];
                _queue[0].Index = 0;
                _queue.RemoveAt(GetSize() - 1);
                Trace.WriteLine(
                    "[Whiteboard] BoardPriorityQueue.Extract: Replaced the top with last element. Calling heapify on root.");
                Heapify(0);

                Trace.WriteLine("[Whiteboard] BoardPriorityQueue.Extract: Returning the extracted out element.");
                Trace.Unindent();
                return queueElement;
            }
        }

        /// <summary>
        ///     Increases the timestamp of given queue element and place it at appropriate position in priority queue.
        /// </summary>
        /// <param name="queueElement">Element to be updated. </param>
        /// <param name="dateTime">The new date-time value. </param>
        public void IncreaseTimestamp(QueueElement queueElement, DateTime dateTime)
        {
            Trace.Indent();
            var index = queueElement.Index;

            // if index is out of range 
            if (index < BoardConstants.EmptySize || index >= GetSize())
            {
                Trace.WriteLine("[Whiteboard] BoardPriorityQueue.IncreaseTimestamp: Index out of range.");
                throw new IndexOutOfRangeException("Element index not in the queue. IncreaseTimestamp failed.");
            }

            if (queueElement.Timestamp > dateTime)
            {
                Trace.WriteLine("[Whiteboard] BoardPriorityQueue.IncreaseTimestamp: Can't decrease timestamp");
                throw new InvalidOperationException();
            }

            // update timestamp and place the element at correct position.
            queueElement.Timestamp = dateTime;
            while (index != 0 && _queue[Parent(index)].Timestamp < _queue[index].Timestamp)
            {
                SwapElements(index, Parent(index));
                index = Parent(index);
            }

            Trace.WriteLine(
                "[Whiteboard] BoardPriorityQueue.IncreaseTimestamp: Increased timestamp and moved to new suitable position.");
            Trace.Unindent();
        }

        /// <summary>
        ///     Deletes the given element from priority queue.
        /// </summary>
        /// <param name="queueElement">Element to be deleted. </param>
        public void DeleteElement(QueueElement queueElement)
        {
            Trace.Indent();

            // Increase the timestamp to max possible value.  
            IncreaseTimestamp(queueElement, DateTime.MaxValue);
            Trace.WriteLine("[Whiteboard] BoardPriorityQueue.DeleteElement: Timestamp for element increased to MAX");

            // extract out max value (highest timestamp)
            Extract();
            Trace.WriteLine("[Whiteboard] BoardPriorityQueue.DeleteElement: Element extracted out.");
            Trace.Unindent();
        }

        /// <summary>
        ///     Removes all elements from priority queue.
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }
    }
}