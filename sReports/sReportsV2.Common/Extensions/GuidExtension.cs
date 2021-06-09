using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Common.Extensions
{
    public static class GuidExtension
    {
        public static string NewGuidStringWithoutDashes()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}