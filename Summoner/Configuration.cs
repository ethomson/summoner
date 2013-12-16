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
        public Configuration(string path)
        {
            string configString = System.IO.File.ReadAllText(path);
            dynamic configJson = SimpleJson.DeserializeObject(configString);

            PollInterval = (int)configJson["poll_interval"];
            ImageCacheDir = DefaultImageCacheDir;
            Clients = LoadDynamic(configJson["clients"]);
            Notifications = LoadDynamic(configJson["notifications"]);
        }

        public string ImageCacheDir
        {
            get;
            set;
        }

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

        public static string DefaultConfigurationPath
        {
            get
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

                return configpath;
            }
        }

        private static string DefaultImageCacheDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Path.Combine(Constants.ApplicationName, "Images"));
            }
        }

        private IEnumerable<Dictionary<string, string>> LoadDynamic(dynamic input)
        {
            List<ConfigurationDictionary> result = new List<ConfigurationDictionary>();

            foreach (dynamic json in input)
            {
                ConfigurationDictionary o = new ConfigurationDictionary(this);

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
