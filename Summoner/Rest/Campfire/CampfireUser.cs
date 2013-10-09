using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest.Campfire
{
    public class CampfireUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string AvatarUrl { get; set; }
    }
}
