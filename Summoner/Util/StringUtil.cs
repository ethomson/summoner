using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summoner.Util
{
    public sealed class StringUtil
    {
        private static readonly char[] hexChars = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        private StringUtil()
        {
        }

        public static string HexString(byte[] value)
        {
            StringBuilder hex = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                hex.Append(hexChars[(value[i] & 0xf0) >> 4]);
                hex.Append(hexChars[(value[i] & 0x0f)]);
            }

            return hex.ToString();
        }
    }
}
