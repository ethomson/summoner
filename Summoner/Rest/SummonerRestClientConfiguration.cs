using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Rest
{
    public class SummonerRestClientConfiguration
    {
        public SummonerRestClientConfiguration()
        {
            UserAgent = Constants.ApplicationName + " (" + Constants.InformationUrl + ")";
        }

        public string UserAgent { get; set; }

        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
