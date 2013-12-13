using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Rest;
using Summoner.Rest.Bandwidth;
using Summoner.Clients;

namespace Summoner.Notifications
{
    public class SmsNotification : Notification
    {
        private readonly ConfigurationDictionary configuration;

        private BandwidthRestClient restClient;
        private string bandwidthUserId;
        private string fromNumber;
        private string toNumber;

        public SmsNotification(ConfigurationDictionary configuration)
        {
            this.configuration = configuration;

            SummonerRestClientConfiguration restClientConfig = new SummonerRestClientConfiguration();
            restClientConfig.Uri = "https://api.catapult.inetwork.com/";
            restClientConfig.Username = configuration["api-token"];
            restClientConfig.Password = configuration["api-secret"];

            bandwidthUserId = configuration["userid"];
            fromNumber = configuration["from"];
            toNumber = configuration["to"];

            restClient = new BandwidthRestClient(restClientConfig);
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
            string notification = String.Format("[{0}:{1}] {2}",
                client.Name, message.Sender, message.Content);
            restClient.SendMessage(bandwidthUserId, fromNumber, toNumber, notification);
        }
    }
}
