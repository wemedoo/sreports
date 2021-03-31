using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SReportsAutorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (request.IsAjaxRequest())
            {
                response.SuppressFormsAuthenticationRedirect = true;
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.End();
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}