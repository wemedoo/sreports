using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using Microsoft.Owin.Security.Cookies;
using sReportsV2.TokenStorage;
using System.Configuration;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SReportsAutorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                var userCookieData = HttpContext.Current.Session["userData"] as UserCookieData;
                var userDAL = DependencyResolver.Current.GetService<IUserDAL>();

                if (!userDAL.IsUserStillValid(userCookieData.Id))
                {
                    SignOutUser(filterContext);
                }
            };

            base.OnAuthorization(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var httpContext = filterContext.HttpContext;
                var request = httpContext.Request;
                var response = httpContext.Response;

                if (request.IsAjaxRequest())
                {
                    //response.SuppressFormsAuthenticationRedirect = true;
                    //response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    //response.End();
                }

                base.HandleUnauthorizedRequest(filterContext);

            }
            else
            {
                //logged and wihout the role to access it - redirect to the custom controller action
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }

        private void SignOutUser(AuthorizationContext filterContext)
        {
            var tokenStore = new SessionTokenStore(null,
                   System.Web.HttpContext.Current, System.Security.Claims.ClaimsPrincipal.Current);

            tokenStore.Clear();
            System.Web.HttpContext.Current.Session["userData"] = null;
            HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            
            //redirect
        }
    }
}