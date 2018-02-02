using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Crypto
{
    public static class Extensions
    {
        public static string ToStringPhrase(this byte[] array)
        {
            return Encoding.ASCII.GetString(array);
        }

        public static byte[] ToBytesArray(this string phrase)
        {
            return Encoding.ASCII.GetBytes(phrase);
        }
    }
}
