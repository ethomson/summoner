using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using Summoner.Rest;
using Summoner.Rest.Tfs;
using Summoner.Util;

namespace Summoner.Clients
{
    public class TfsClient : Client
    {
        private readonly ConfigurationDictionary configuration;

        private readonly SummonerRestClientConfiguration restConfiguration = new SummonerRestClientConfiguration();
        private readonly string roomName;

        private readonly ImageManager imageManager;

        private readonly Object runningLock = new Object();
        private bool running;

        private TfsTeamRoomRestClient restClient;
        private TfsTeamRoom room;
        private DateTime lastMessageTime;
        private int hwm;

        private readonly Dictionary<Guid, string> userNames = new Dictionary<Guid, string>();

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

            imageManager = new ImageManager(this);
        }

        public ConfigurationDictionary Configuration
        {
            get
            {
                return configuration;
            }
        }

        public bool Running
        {
            get
            {
                lock (runningLock)
                {
                    return running;
                }
            }
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

        public void Start()
        {
            lock (runningLock)
            {
                if (running)
                {
                    Stop();
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
                IEnumerable<TfsTeamRoomMessage> messages = restClient.Messages(room);

                TfsTeamRoomMessage lastMessage = (messages.Count() > 0) ? restClient.Messages(room).Last() : null;

                lastMessageTime = (lastMessage != null) ? lastMessage.PostedTime : default(DateTime);
                hwm = (lastMessage != null) ? lastMessage.Id : 0;

                running = true;
            }

            new Thread(delegate()
            {
                imageManager.Start();
            }).Start();
        }

        private string ResolveUserId(Guid userTfId)
        {
            lock (userNames)
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
        }

        private string ResolveUserImage(Guid userTfId)
        {
            string imageUrl = String.Format("{0}/_api/_common/IdentityImage?id={1}&__v=5",
                configuration["uri"], userTfId.ToString());

            return imageManager.GetImage(imageUrl);
        }

        public IEnumerable<Message> RecentMessages()
        {
            lock (runningLock)
            {
                if (!running)
                {
                    throw new Exception("Not connected");
                }

                List<Message> messages = new List<Message>();
                IEnumerable<TfsTeamRoomMessage> teamMessages = restClient.Messages(room, "PostedTime gt " + lastMessageTime.ToUniversalTime().ToString(RestSharp.DateFormat.Iso8601));

                foreach (TfsTeamRoomMessage teamMessage in teamMessages)
                {
                    lastMessageTime = teamMessage.PostedTime;

                    if (teamMessage.Id <= hwm)
                    {
                        continue;
                    }

                    hwm = teamMessage.Id;

                    messages.Add(new Message()
                    {
                        Sender = ResolveUserId(teamMessage.PostedByUserTfid),
                        SenderImage = ResolveUserImage(teamMessage.PostedByUserTfid),
                        Time = teamMessage.PostedTime,
                        Content = teamMessage.Content
                    });
                }

                return messages;
            }
        }

        public void Stop()
        {
            lock (runningLock)
            {
                restClient = null;
                room = null;
                lastMessageTime = default(DateTime);
                hwm = 0;

                running = false;
            }

            imageManager.Stop();
        }
    }
}
