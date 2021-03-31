using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.Common.Extensions
{
    public static class StringExtension
    {
        public static bool IsObjectId(this string id)
        {
            bool isHexa = System.Text.RegularExpressions.Regex.IsMatch(id, @"\A\b[0-9a-fA-F]+\b\Z");
            return isHexa && id.Length.Equals(24);
        }
    }
}
