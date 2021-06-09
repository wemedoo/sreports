using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Extensions;
using System;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters = Ensure.IsNotNull(filters, nameof(filters));
            filters.Add(new HandleErrorAttribute());
        }
    }
}
