namespace Networking
{
    public interface IQueue
    {
        /// <summary>
        /// Size of the queue
        /// </summary>
        public int Size();
        
        /// <summary>
        /// Dequeues all the elements
        /// </summary>
        public void Clear(); 

        /// <summary>
        /// Enqueues an object of IPacket
        /// </summary>
        public void Enqueue(Packet item);
        
        /// <summary>
        /// Dequeues an item from the queue and returns the item
        /// </summary>
        public Packet Dequeue(); 
        
        /// <summary>
        /// Peeks into the first element of the queue
        /// </summary>
        public Packet Peek(); 
        
        /// <summary>
        /// Checks if the queue is empty or not
        /// </summary>
        public bool IsEmpty(); 
    }
}



