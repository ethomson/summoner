using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner
{
    public class ConfigurationDictionary : Dictionary<string, string>
    {
        public ConfigurationDictionary(Configuration global)
        {
            Global = global;
        }

        public Configuration Global
        {
            get;
            private set;
        }

        public bool IsTrue(string key)
        {
            return (ContainsKey(key) &&
                (this[key].Equals("true", StringComparison.OrdinalIgnoreCase) ||
                this[key].Equals("yes", StringComparison.OrdinalIgnoreCase)));

        }

        public bool IsFalse(string key)
        {
            return (ContainsKey(key) &&
                (this[key].Equals("false", StringComparison.OrdinalIgnoreCase) ||
                this[key].Equals("no", StringComparison.OrdinalIgnoreCase)));

        }
    }
}
