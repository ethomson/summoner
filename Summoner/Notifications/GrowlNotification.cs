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

        private ConfigurationDictionary configuration;

        private readonly GrowlConnector growlConnector;

        public GrowlNotification(ConfigurationDictionary configuration)
        {
            this.configuration = configuration;

            growlConnector = new GrowlConnector();

            Application growlApplication = new Application(Constants.ApplicationName);

            growlConnector.Register(growlApplication, new NotificationType[] {
                new NotificationType(NotificationName, "Incoming Message")
            });
        }

        public ConfigurationDictionary Configuration
        {
            get
            {
                return configuration;
            }
        }

        public void Notify(Client client, Message message)
        {
            growlConnector.Notify(new Growl.Connector.Notification(
                Constants.ApplicationName,
                NotificationName,
                "",
                String.Format("Notification in {0}", client.Name),
                String.Format("[{0}] {1}", message.Sender, message.Content)));
        }
    }
}
