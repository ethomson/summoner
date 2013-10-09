using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Summoner.Rest.Bandwidth
{
    public class BandwidthRestClient : SummonerRestClient
    {
        public BandwidthRestClient(SummonerRestClientConfiguration config)
            : base(config)
        {
        }

        public void SendMessage(string userId, string from, string to, string text)
        {
            var request = new RestRequest("/v1/users/{UserId}/messages", Method.POST);
            request.AddUrlSegment("UserId", userId);

            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { from = from, to = to, text = text });

            Execute(request);
        }
    }
}
