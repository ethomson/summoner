using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest.Campfire
{
    public class CampfireRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public bool Locked { get; set; }
        public int MembershipLimit { get; set; }
        public bool Full { get; set; }
        public bool OpenToGuests { get; set; }
        public string ActiveTokenValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
