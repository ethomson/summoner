using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Summoner.Rest.Tfs
{
    public class TfsTeamRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsClosed { get; set; }
        public Guid CreatorUserTfId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasAdminPermissions { get; set; }
        public bool HasReadWritePermissions { get; set; }
    }
}
