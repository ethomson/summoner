using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Rest;
using Summoner.Rest.Campfire;
using Summoner.Util;

namespace Summoner.Clients
{
    public class CampfireClient : Client
    {
        private readonly Dictionary<string, string> configuration;

        private readonly SummonerRestClientConfiguration restConfiguration = new SummonerRestClientConfiguration();
        private string roomName;

        private CampfireRestClient restClient;
        private CampfireRoom room;
        private int? hwm = null;

        private Dictionary<int, string> userNames = new Dictionary<int, string>();

        public CampfireClient(Dictionary<string, string> configuration)
        {
            if (configuration == null || !configuration.ContainsKey("uri"))
            {
                throw new Exception("Invalid configuration: missing uri");
            }

            this.configuration = configuration;

            restConfiguration.Uri = configuration["uri"];
            restConfiguration.Username = configuration["api-token"];
            restConfiguration.Password = "X";
            roomName = configuration["room"];
        }

        public Dictionary<string, string> Configuration
        {
            get
            {
                return configuration;
            }
        }

        public bool Connected
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                if (Configuration.ContainsKey("name"))
                {
                    return Configuration["name"];
                }

                if (Configuration.ContainsKey("room"))
                {
                    return Configuration["room"];
                }

                return Configuration["type"];
            }
        }

        public void Connect()
        {
            if (Connected)
            {
                Close();
            }

            restClient = new CampfireRestClient(restConfiguration);
            room = null;

            foreach (CampfireRoom r in restClient.Rooms())
            {
                if (roomName.Equals(r.Name))
                {
                    room = r;
                    break;
                }
            }

            if (room == null)
            {
                throw new Exception(String.Format("Could not find room '{0}'", roomName));
            }

            CampfireMessage lastMessage = restClient.Messages(room).Last();

            if (lastMessage != null)
            {
                hwm = lastMessage.Id;
            }

            Connected = true;
        }

        private string ResolveUserId(int userId)
        {
            if (!userNames.ContainsKey(userId))
            {
                CampfireUser user = restClient.User(userId);

                if (user != null)
                {
                    userNames[userId] = user.Name;
                }
                else
                {
                    userNames[userId] = "Unknown user " + userId.ToString();
                }
            }

            return userNames[userId];
        }

        public IEnumerable<Message> RecentMessages()
        {
            if (!Connected)
            {
                throw new Exception("Not connected");
            }

            CampfireRestClient.CampfireMessageOptions messageOptions =
                (hwm != null) ?
                new CampfireRestClient.CampfireMessageOptions() { SinceMessageId = hwm } :
                null;

            IEnumerable<CampfireMessage> campfireMessages = restClient.Messages(room, messageOptions);
            List<Message> messages = new List<Message>();

            foreach (CampfireMessage campfireMessage in campfireMessages)
            {
                hwm = campfireMessage.Id;

                if (!campfireMessage.Type.Equals("TextMessage"))
                {
                    continue;
                }

                int userId = campfireMessage.UserId;

                messages.Add(new Message()
                {
                    Sender = ResolveUserId(campfireMessage.UserId),
                    Time = campfireMessage.CreatedAt,
                    Content = campfireMessage.Body
                });
            }

            return messages;
        }

        public void Close()
        {
            restClient = null;
            room = null;
            userNames.Clear();

            Connected = false;
        }
    }
}
