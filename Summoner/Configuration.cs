using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using RestSharp;

namespace Summoner
{
    public class Configuration
    {
        public int PollInterval
        {
            get;
            set;
        }

        public IEnumerable<ConfigurationDictionary> Clients
        {
            get;
            set;
        }

        public IEnumerable<ConfigurationDictionary> Notifications
        {
            get;
            set;
        }

        public static Configuration Load()
        {
            string exe = System.Reflection.Assembly.GetEntryAssembly().Location;
            string configpath;

            if (exe.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                configpath = exe.Substring(0, exe.Length - 4) + ".config";
            }
            else
            {
                configpath = exe + ".config";
            }

            return Load(configpath);
        }

        public static Configuration Load(string path)
        {
            string configString = System.IO.File.ReadAllText(path);
            dynamic configJson = SimpleJson.DeserializeObject(configString);

            Configuration config = new Configuration();

            config.PollInterval = (int)configJson["poll_interval"];
            config.Clients = LoadDynamic(configJson["clients"]);
            config.Notifications = LoadDynamic(configJson["notifications"]);

            return config;
        }

        private static IEnumerable<Dictionary<string, string>> LoadDynamic(dynamic input)
        {
            List<ConfigurationDictionary> result = new List<ConfigurationDictionary>();

            foreach (dynamic json in input)
            {
                ConfigurationDictionary o = new ConfigurationDictionary();

                foreach (KeyValuePair<string, dynamic> prop in json)
                {
                    o[prop.Key] = prop.Value.ToString();
                }

                result.Add(o);
            }

            return result;
        }
    }
}
