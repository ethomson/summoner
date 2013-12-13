using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Rest;
using Summoner.Rest.Tfs;
using Summoner.Util;

namespace Summoner.Clients
{
    public class TfsClient : Client
    {
        private readonly ConfigurationDictionary configuration;

        private readonly SummonerRestClientConfiguration restConfiguration = new SummonerRestClientConfiguration();
        private string roomName;

        private TfsTeamRoomRestClient restClient;
        private TfsTeamRoom room;
        private TfsTeamRoomMessage lastMessage;
        private int hwm;

        private Dictionary<Guid, string> userNames = new Dictionary<Guid, string>();

        public TfsClient(ConfigurationDictionary configuration)
        {
            if (configuration == null || !configuration.ContainsKey("uri"))
            {
                throw new Exception("Invalid configuration: missing uri");
            }

            this.configuration = configuration;

            restConfiguration.Uri = configuration["uri"];
            restConfiguration.Username = configuration["username"];
            restConfiguration.Password = configuration["password"];
            roomName = configuration["room"];
        }

        public ConfigurationDictionary Configuration
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

            restClient = new TfsTeamRoomRestClient(restConfiguration);
            room = null;

            foreach (TfsTeamRoom r in restClient.Rooms())
            {
                if (roomName.Equals(r.Name))
                {
                    room = r;
                    break;
                }
            }

            if (room == null)
            {
                throw new Exception(String.Format("Could not find team room '{0}'", roomName));
            }

            /* Load the high-water mark */
            lastMessage = restClient.Messages(room).Last();
            DateTime filter = (lastMessage != null) ? lastMessage.PostedTime : default(DateTime);
            hwm = (lastMessage != null) ? lastMessage.Id : 0;

            Connected = true;
        }

        private string ResolveUserId(Guid userTfId)
        {
            if (!userNames.ContainsKey(userTfId))
            {
                TfsUserProfileRestClient profileClient = new TfsUserProfileRestClient(restConfiguration);
                TfsUserIdentity identity = profileClient.GetIdentity(userTfId.ToString());

                if (identity != null)
                {
                    userNames[userTfId] = identity.DisplayName;
                }
                else
                {
                    userNames[userTfId] = "Unknown user " + userTfId.ToString();
                }
            }

            return userNames[userTfId];
        }

        public IEnumerable<Message> RecentMessages()
        {
            if (!Connected)
            {
                throw new Exception("Not connected");
            }

            List<Message> messages = new List<Message>();
            IEnumerable<TfsTeamRoomMessage> teamMessages = restClient.Messages(room, "PostedTime gt " + lastMessage.PostedTime.ToUniversalTime().ToString(RestSharp.DateFormat.Iso8601));

            foreach (TfsTeamRoomMessage teamMessage in teamMessages)
            {
                lastMessage = teamMessage;

                if (teamMessage.Id <= hwm)
                {
                    continue;
                }

                hwm = teamMessage.Id;

                messages.Add(new Message() {
                    Sender = ResolveUserId(teamMessage.PostedByUserTfid),
                    Time = teamMessage.PostedTime,
                    Content = teamMessage.Content
                });
            }

            return messages;
        }

        public void Close()
        {
            restClient = null;
            room = null;
            lastMessage = null;
            hwm = 0;

            Connected = false;
        }
    }
}
