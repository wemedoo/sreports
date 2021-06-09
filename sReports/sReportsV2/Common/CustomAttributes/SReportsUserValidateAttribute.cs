using Serilog;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Common.CustomAttributes
{
    public class SReportsUserValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));

            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var allErrors = filterContext.Controller.ViewData.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
    }
}