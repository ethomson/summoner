using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Summoner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string configPath = null;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/config:") || arg.StartsWith("/config="))
                {
                    configPath = arg.Substring(8);
                    continue;
                }

                throw new Exception(String.Format("Invalid argument: {0}", arg));
            }

            Configuration config = new Configuration(
                (configPath != null) ? configPath : Configuration.DefaultConfigurationPath);

            Summoner summoner = new Summoner(config);

            summoner.Start();
            summoner.Stop();

            return 0;
        }
    }
}
