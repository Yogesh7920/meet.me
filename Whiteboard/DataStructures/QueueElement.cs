/**
 * Owned By: Ashish Kumar Gupta
 * Created By: Ashish Kumar Gupta
 * Date Created: 10/25/2021
 * Date Modified: 10/25/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    /// <summary>
    /// The element to store in priority queue. 
    /// </summary>
    public class QueueElement
    {
        /// <summary>
        /// Constructor for the class. 
        /// </summary>
        /// <param name="id">Id of the element.</param>
        /// <param name="dateTime">Time of last update of the BoardShape with Id id.</param>
        /// <param name="index">Index of the element.</param>
        public QueueElement(string id, DateTime dateTime, int index = 0)
        {
            Id = id;
            Timestamp = dateTime;
            Index = index;
        }

        /// <summary>
        /// Getter and setter for Id. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Getter and setter for Timestamp. 
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Getter and setter for Index. 
        /// </summary>
        public int Index { get; set; }
    }
}
