using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Summoner.Rest.Tfs
{
    public class TfsUserProfileRestClient : SummonerRestClient
    {
        public TfsUserProfileRestClient(SummonerRestClientConfiguration config)
            : base(config)
        {
        }

        public TfsUserIdentity GetIdentity()
        {
            return Execute<TfsUserIdentityContainer>(new RestRequest("/_api/_common/GetUserProfile")).Identity;
        }

        public TfsUserIdentity GetIdentity(string tfUserId)
        {
            var request = new RestRequest("/_api/_common/GetUserProfile/{TfUserId}");
            request.AddUrlSegment("TfUserId", tfUserId);
            return Execute<TfsUserIdentityContainer>(request).Identity;
        }

        public class TfsUserIdentityContainer
        {
            public TfsUserIdentity Identity { get; set; }
        }
    }
}
