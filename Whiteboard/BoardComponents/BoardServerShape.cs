/**
 * Owned By: Parul Sangwan
 * Created By: Parul Sangwan
 * Date Created: 10/11/2021
 * Date Modified: 10/11/2021
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whiteboard
{
    public class BoardServerShape
    {
        public MainShape MainShapeDefiner { get; set; }
        public int UserLevel { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string Uid { get; set; }
        public string ShapeOwnerId { get; set; }
        public Operation RecentOperation { get; set; }
        public int CheckPointNumber { get; set; }
    }
}
