using System.Collections.Generic;
using System.Linq;

namespace SCNRWeb.Helper
{
    public static class StringExtensions
    {
        public static string TruncatePretty(this string str, int maxChars)
        {
            if (str == null)
                return "";

            if (str.Length <= maxChars)
                return str;

            return str.Substring(0, str.Substring(0, maxChars - 2).LastIndexOf(' ')) + "...";
        }

        public static IEnumerable<int> IndexOfAll(this string str, char c)
        {
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == c)
                    yield return i;
            }
        }
    }
}
