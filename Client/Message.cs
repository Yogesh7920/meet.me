using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string TextMessage { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public bool tofrom { get; set; }
    }
}
