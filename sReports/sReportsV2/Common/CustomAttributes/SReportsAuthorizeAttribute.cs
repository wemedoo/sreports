using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using Microsoft.Owin.Security.Cookies;
using sReportsV2.TokenStorage;
using System.Net;
using sReportsV2.Common.Constants;

namespace sReportsV2.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SReportsAuthorizeAttribute : AuthorizeAttribute, IResultFilter
    {
        public string Module { get; set; }
        public string Permission { get; set; }

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
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                HandleSReportsUnauthorizedRequest(filterContext);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }  
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return DoesUserHaveAccessRight() && base.AuthorizeCore(httpContext);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string actionName = (string)filterContext.RouteData.Values["action"];
            string requestType = filterContext.HttpContext.Request.RequestType;
            if (requestType == "GET")
            {
                if (actionName == "Create")
                {
                    filterContext.Controller.ViewBag.ReadOnly = false;
                    filterContext.Controller.ViewBag.Disabled = "";
                }
                else if (actionName == "Edit")
                {
                    filterContext.Controller.ViewBag.ReadOnly = !DoesUserHaveAccessRight(Module, PermissionNames.CreateUpdate);
                    filterContext.Controller.ViewBag.Disabled = filterContext.Controller.ViewBag.ReadOnly ? "disabled" : "";
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
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

        private void HandleSReportsUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            if (request.IsAjaxRequest())
            {
                var response = httpContext.Response;
                response.SuppressFormsAuthenticationRedirect = true;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                string forbiddenMessage = FormatForbiddenMessage(filterContext.ActionDescriptor.ActionName);
                response.StatusDescription = forbiddenMessage;
                response.Write(forbiddenMessage);
                response.End();
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", id = $"{Permission}_{Module}" }));
            }
        }

        private string FormatForbiddenMessage(string actionName)
        {
            return string.Format("{0} ({1} : {2}, {3} : {4}, {5} : {6})", Resources.TextLanguage.Access_Denied_Message, Resources.TextLanguage.Permission, Permission, Resources.TextLanguage.Module, Module, Resources.TextLanguage.Action, actionName);
        }

        private bool DoesUserHaveAccessRight()
        {
            return DoesUserHaveAccessRight(Module, Permission);
        }

        private bool DoesUserHaveAccessRight(string module, string permission)
        {
            if (!string.IsNullOrWhiteSpace(module) && !string.IsNullOrWhiteSpace(permission))
            {
                UserCookieData userCookieData = HttpContext.Current.Session["userData"] as UserCookieData;
                if (userCookieData != null)
                {
                    if (!userCookieData.UserHasPermission(permission, module))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}