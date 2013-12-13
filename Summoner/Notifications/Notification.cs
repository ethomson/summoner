using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public interface Notification
    {
        ConfigurationDictionary Configuration { get; }
        void Notify(Client client, Message message);
    }
}
