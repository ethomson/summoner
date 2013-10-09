using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Util;

namespace Summoner.Clients
{
    public class ClientFactory
    {
        public static Client NewClient(Dictionary<string, string> configuration)
        {
            Assert.NotNull(configuration, "configuration");

            if (!configuration.ContainsKey("type"))
            {
                throw new Exception("Invalid client configuration (no type)");
            }

            if (configuration["type"].Equals("campfire"))
            {
                return new CampfireClient(configuration);
            }
            else if (configuration["type"].Equals("tfs"))
            {
                return new TfsClient(configuration);
            }

            throw new UnknownClientTypeException(String.Format("Unknown client type: {0}", configuration["type"]));
        }
    }

    public class UnknownClientTypeException : Exception
    {
        public UnknownClientTypeException(string message)
            : base(message)
        {
        }
    }
}
