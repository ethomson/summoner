using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest.Tfs
{
    public enum TfsTeamRoomMessageType
    {
        System, Normal
    }

    public class TfsTeamRoomMessage
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public TfsTeamRoomMessageType MessageType { get; set; }
        public DateTime PostedTime { get; set; }
        public int PostedRoomId { get; set; }
        public Guid PostedByUserTfid { get; set; }
    }
}
