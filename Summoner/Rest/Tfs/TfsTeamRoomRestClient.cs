using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Summoner.Rest.Tfs
{
    public class TfsTeamRoomRestClient : SummonerRestClient
    {
        public TfsTeamRoomRestClient(SummonerRestClientConfiguration config)
            : base(config)
        {
        }

        public IEnumerable<TfsTeamRoom> Rooms()
        {
            return Execute<TfsTeamRoomList>(new RestRequest("/_apis/chat/rooms")).Value;
        }

        public void Join(TfsTeamRoom room)
        {
            var userProfileClient = new TfsUserProfileRestClient(Config);
            var identity = userProfileClient.GetIdentity();

            var request = new RestRequest("/_apis/chat/rooms/{RoomId}/users/{Identity}", Method.PUT);
            request.AddUrlSegment("RoomId", room.Id.ToString());
            request.AddUrlSegment("Identity", identity.TeamFoundationId);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { userId = identity.TeamFoundationId });
            Execute(request);
        }

        public void Write(TfsTeamRoom room, string message)
        {
            var request = new RestRequest("/_apis/chat/rooms/{RoomId}/messages", Method.POST);
            request.AddUrlSegment("RoomId", room.Id.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { content = message });
            Execute(request);
        }

        public IEnumerable<TfsTeamRoomMessage> Messages(TfsTeamRoom room, string filter = null)
        {
            var request = new RestRequest("/_apis/chat/rooms/{RoomId}/messages", Method.GET);
            request.AddUrlSegment("RoomId", room.Id.ToString());

            if (filter != null)
            {
                request.AddParameter("$filter", filter);
            }

            TfsTeamMessageList messages = Execute<TfsTeamMessageList>(request);
            return (messages != null) ? messages.Value : null;
        }

        public void Leave(TfsTeamRoom room)
        {
            var userProfileClient = new TfsUserProfileRestClient(Config);
            var identity = userProfileClient.GetIdentity();

            var request = new RestRequest("/_apis/chat/rooms/{RoomId}/users/{Identity}", Method.DELETE);
            request.AddUrlSegment("RoomId", room.Id.ToString());
            request.AddUrlSegment("Identity", identity.TeamFoundationId);
            request.RequestFormat = DataFormat.Json;
            Execute(request);
        }

        private class TfsTeamRoomList
        {
            public int Count { get; set; }
            public List<TfsTeamRoom> Value { get; set; }
        }

        private class TfsTeamMessageList
        {
            public int Count { get; set; }
            public List<TfsTeamRoomMessage> Value { get; set; }
        }
    }
}
