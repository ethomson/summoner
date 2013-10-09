using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Clients
{
    public interface Client
    {
        Dictionary<string, string> Configuration { get; }
        bool Connected { get; }
        string Name { get; }
        void Connect();
        IEnumerable<Message> RecentMessages();
        void Close();
    }
}
