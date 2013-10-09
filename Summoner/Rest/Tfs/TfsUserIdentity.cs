using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest.Tfs
{
    public class TfsUserIdentity
    {
        public string IdentityType { get; set; }
        public string DisplayName { get; set; }
        public string FriendlyDisplayName { get; set; }
        public string SubHeader { get; set; }
        public string TeamFoundationId { get; set; }
        public string AccountName { get; set; }
    }
}
