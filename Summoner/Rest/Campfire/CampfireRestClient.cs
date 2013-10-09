using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Summoner.Rest.Campfire
{
    public class CampfireRestClient : SummonerRestClient
    {
        public CampfireRestClient(SummonerRestClientConfiguration config) : base(config)
        {
        }

        public IEnumerable<CampfireRoom> Rooms()
        {
            return Execute<CampfireRoomList>(new RestRequest("/rooms.json")).Rooms;
        }

        public IEnumerable<CampfireMessage> Messages(CampfireRoom room, CampfireMessageOptions options = null)
        {
            var request = new RestRequest("/room/{RoomId}/recent.json", Method.GET);
            request.AddUrlSegment("RoomId", room.Id.ToString());

            if (options != null && options.Limit != null)
            {
                request.AddParameter("limit", options.Limit);
            }

            if (options != null && options.SinceMessageId != null)
            {
                request.AddParameter("since_message_id", options.SinceMessageId);
            }

            return Execute<CampfireMessageList>(request).Messages;
        }

        public CampfireUser User(int? id = null)
        {
            var request = new RestRequest("/users/{UserId}.json", Method.GET);
            request.AddUrlSegment("UserId", (id != null) ? id.ToString() : "me");
            CampfireUserContainer container = Execute<CampfireUserContainer>(request);

            if (container == null)
            {
                return null;
            }

            return container.User;
        }

        public class CampfireMessageOptions
        {
            public CampfireMessageOptions()
            {
                Limit = null;
                SinceMessageId = null;
            }

            public int? Limit { get; set; }
            public int? SinceMessageId { get; set; }
        }

        private class CampfireRoomList
        {
            public List<CampfireRoom> Rooms { get; set; }
        }

        private class CampfireMessageList
        {
            public List<CampfireMessage> Messages { get; set; }
        }

        private class CampfireUserContainer
        {
            public CampfireUser User { get; set; }
        }
    }
}
