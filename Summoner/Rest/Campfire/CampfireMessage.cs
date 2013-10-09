using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest.Campfire
{
    public class CampfireMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Starred { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
    }
}
