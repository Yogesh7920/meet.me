/// <author>Alisetti Sai Vamsi</author>
/// <created>13/10/2021</created>
/// <summary>
/// This file contains the definition of the interface IQueue.
/// </summary>

namespace Networking
{
    public interface IQueue
    {
        /// <summary>
        ///     Register Module into the multilevel queue.
        /// </summary>
        /// <param name="moduleId">Unique Id for module.</param>
        /// <param name="priority">Priority Number indicating the weight to be given to the module.</param>
        public void RegisterModule(string moduleId, int priority);

        /// <summary>
        ///     Size of the queue.
        /// </summary>
        /// <returns>The number of packets the queue holds.</returns>
        public int Size();

        /// <summary>
        ///     Dequeues all the elements
        /// </summary>
        public void Clear();

        /// <summary>
        ///     Enqueues an object of IPacket.
        /// </summary>
        /// <param name="packet">Reference to the packet that has to be enqueued</param>
        public void Enqueue(Packet packet);

        /// <summary>
        ///     Dequeues an item from the queue and returns the item.
        /// </summary>
        /// <returns>Returns the dequeued packet from the queue.</returns>
        /// <exception cref="Exception">Cannot dequeue an empty queue</exception>
        public Packet Dequeue();

        /// <summary>
        ///     Peeks into the first element of the queue.
        /// </summary>
        /// <returns>Returns the peeked packet from the queue.</returns>
        /// <exception cref="Exception">Cannot peek an empty queue</exception>
        public Packet Peek();

        /// <summary>
        ///     Checks if the queue is empty or not.
        /// </summary>
        /// <returns>True if queue is empty and false otherwise.</returns>
        public bool IsEmpty();

        /// <summary>
        ///     Blocks until the queue has at-least one packet.
        /// </summary>
        public void WaitForPacket();

        /// <summary>
        ///     Closes the WaitHandle for this queue and clears all the resources held by it. 
        /// </summary>
        public void Close();
    }
}