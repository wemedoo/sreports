using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace sReportsV2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config = Ensure.IsNotNull(config, nameof(config));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
