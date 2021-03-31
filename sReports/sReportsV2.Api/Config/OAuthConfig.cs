using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web.Resource;
using sReportsV2.Api.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.Config
{
    public class OAuthConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                .AddAzureADBearer(options => configuration.Bind("AzureAd", options));

            services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {
                // This is a Microsoft identity platform web API.
                options.Authority += "/v2.0";

                // The web API accepts as audiences both the Client ID (options.Audience) and api://{ClientID}.
                options.TokenValidationParameters.ValidAudiences = new[]
                {
                 options.Audience,
                 $"api://{options.Audience}"
                };

                // Instead of using the default validation (validating against a single tenant,
                // as we do in line-of-business apps),
                // we inject our own multitenant validation logic (which even accepts both v1 and v2 tokens).
                options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.GetIssuerValidator(options.Authority).Validate;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessAsApplication", policy => policy.Requirements.Add(new HasAnyAcceptedScopeRequirement("access_as_application")));
                options.AddPolicy("AccessAsDfDApplication", policy => policy.Requirements.Add(new HasAnyAcceptedScopeRequirement("access_as_dfd_application")));
            });

            services.AddSingleton<IAuthorizationHandler, HasAnyAcceptedScopeHandler>();
        }
    }
}
