using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Growl;
using Growl.Connector;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public class GrowlNotification : Notification
    {
        private const string NotificationName = "MESSAGE";

        private Dictionary<string, string> configuration;

        private readonly GrowlConnector growlConnector;

        public GrowlNotification(Dictionary<string, string> configuration)
        {
            this.configuration = configuration;

            growlConnector = new GrowlConnector();

            Application growlApplication = new Application(SummonerConstants.ApplicationName);

            growlConnector.Register(growlApplication, new NotificationType[] {
                new NotificationType(NotificationName, "Incoming Message")
            });
        }

        public Dictionary<string, string> Configuration
        {
            get
            {
                return configuration;
            }
        }

        public void Notify(Client client, Message message)
        {
            growlConnector.Notify(new Growl.Connector.Notification(
                SummonerConstants.ApplicationName,
                NotificationName,
                "",
                String.Format("Notification in {0}", client.Name),
                String.Format("[{0}] {1}", message.Sender, message.Content)));
        }
    }
}
