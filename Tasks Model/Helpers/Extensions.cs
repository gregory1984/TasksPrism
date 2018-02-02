using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Tasks_Model.Helpers
{
    public static class ExtMethods
    {
        public static bool IsNull(this string phrase)
        {
            return string.IsNullOrWhiteSpace(phrase);
        }

        public static bool IsNotNull(this string phrase)
        {
            return !string.IsNullOrWhiteSpace(phrase);
        }

        public static bool IsNull(this DateTime? datetime)
        {
            return datetime == null;
        }

        public static bool IsNotNull(this DateTime? datetime)
        {
            return datetime != null;
        }

        public static bool IsNull(this TimeSpan? timespan)
        {
            return timespan == null;
        }

        public static bool IsNotNull(this TimeSpan? timespan)
        {
            return timespan != null;
        }

        public static bool IsNull(this int? number)
        {
            return number == null;
        }

        public static bool IsNotNull(this int? number)
        {
            return number != null;
        }
    }
}
