using System;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Common.Extensions
{
    public static class HtmlExtension
    {
        public static IHtmlString ReadOnly(this HtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "readonly" : "");
        }

        public static IHtmlString Disabled(this HtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "disabled" : "");
        }

        public static IHtmlString DateTimeDisabled(this HtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"openDateTimeDatePicker(event)\"");
        }

        public static IHtmlString DateDisabled(this HtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"showDatePicker(event)\"");
        }

        public static IHtmlString TimeDisabled(this HtmlHelper helper, bool? readOnly)
        {
            helper = Ensure.IsNotNull(helper, nameof(helper));
            return helper.Raw(readOnly.HasValue && readOnly.Value ? "" : "onclick=\"openDateTimeTimePicker(event)\"");
        }
    }
}