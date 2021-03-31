using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Serilog;
using sReportsV2.Configs;
using sReportsV2.Domain.Mongo;
using sReportsV2.MapperProfiles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace sReportsV2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(x =>
            {
                x.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            });
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperWebConfiguration.Configure();
            SerilogConfiguration.ConfigureWritingToFile();
            MongoConfiguration.ConnectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];

            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sr");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("sr");
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            // retrieve roles from UserData
            string[] roles = authTicket.UserData.Split(';');

            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
        }
    }
}
