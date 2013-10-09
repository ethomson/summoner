using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner
{
    public class Message
    {
        public string Sender { get; internal set; }
        public DateTime Time { get; internal set; }
        public string Content { get; internal set; }
    }
}
