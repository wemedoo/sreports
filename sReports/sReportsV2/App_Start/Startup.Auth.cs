using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using sReportsV2.Common.MicrosoftAuthentificationHelper;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Models.MicrosoftAuthentification;
using sReportsV2.TokenStorage;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Microsoft.Owin.Security.Google;
using Owin.Security.Providers.LinkedIn;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;
using System.Web.Mvc;

namespace sReportsV2
{
    public partial class Startup
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["AADAppId"];
        private static string appSecret = ConfigurationManager.AppSettings["AADAppSecret"];
        private static string graphScopes = ConfigurationManager.AppSettings["AADAppScopes"];

        private static string googleClientId = ConfigurationManager.AppSettings["GoogleClientId"];
        private static string googleClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];

        private static string protocol = ConfigurationManager.AppSettings["protocol"];
        public void ConfigureAuth(IAppBuilder app)
        {

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/User/Login"),
                LogoutPath = new PathString("/User/Logout"),
                ExpireTimeSpan = TimeSpan.FromMinutes(20)
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = appId,
                    Authority = "https://login.microsoftonline.com/common/v2.0",
                    Scope = $"openid email profile offline_access {graphScopes}",
                    RedirectUri = $"{protocol}://{HttpContext.Current.Request.Url.Authority}/",
                    PostLogoutRedirectUri = $"{protocol}://{HttpContext.Current.Request.Url.Authority}/",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        // For demo purposes only, see below
                        ValidateIssuer = false

                        // In a real multi-tenant app, you would add logic to determine whether the
                        // issuer was from an authorized tenant
                        //ValidateIssuer = true,
                        //IssuerValidator = (issuer, token, tvp) =>
                        //{
                        //    if (!string.IsNullOrWhiteSpace(issuer))
                        //    {
                        //        return issuer;
                        //    }
                        //    else
                        //    {
                        //        throw new SecurityTokenInvalidIssuerException("Invalid issuer");
                        //    }
                        //}
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailedAsync,
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync,
                        RedirectToIdentityProvider = async (context) =>
                        {
                            context.ProtocolMessage.RedirectUri = $"{protocol}://{HttpContext.Current.Request.Url.Authority}/";
                        }
                    },
                    CookieManager = new SameSiteCookieManager(
                                     new SystemWebCookieManager())

                }
            );

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret,
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = OnGoogleAuthenticatedAsync,
                    OnReturnEndpoint = OnGoogleReturnAsync
                },
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieManager = new SameSiteCookieManager(
                                         new SystemWebCookieManager())

            });

        }

            
        
        private Task OnGoogleAuthenticatedAsync(GoogleOAuth2AuthenticatedContext context)
        {
            AddUserIfNotExist(new CachedUser() 
            {
                Email = context.User.Value<string>("email"),
                FirstName = context.User.Value<string>("given_name"),
                LastName = context.User.Value<string>("family_name")
            }, GlobalUserSource.Google);

           

            return Task.FromResult(0);
        }
        private Task OnGoogleReturnAsync(GoogleOAuth2ReturnEndpointContext context)
        {
            return Task.FromResult(0);
        }

        private static Task OnAuthenticationFailedAsync(AuthenticationFailedNotification<OpenIdConnectMessage,OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            string redirect = $"/User/Error?message={notification.Exception.Message}";
            if (notification.ProtocolMessage != null && !string.IsNullOrEmpty(notification.ProtocolMessage.ErrorDescription))
            {
                redirect += $"&debug={notification.ProtocolMessage.ErrorDescription}";
            }
            notification.Response.Redirect(redirect);
            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                .WithRedirectUri($"{protocol}://{HttpContext.Current.Request.Url.Authority}/")
                .WithClientSecret(appSecret)
                .Build();

            var signedInUser = new ClaimsPrincipal(notification.AuthenticationTicket.Identity);
            var tokenStore = new SessionTokenStore(idClient.UserTokenCache, HttpContext.Current, signedInUser);

            try
            {

                string[] scopes = graphScopes.Split(' ');

                var result = await idClient.AcquireTokenByAuthorizationCode(
                    scopes, notification.Code).ExecuteAsync();

                var userDetails = await GraphHelper.GetUserDetailsAsync(result.AccessToken);
                tokenStore.SaveUserDetails(userDetails);
                AddUserIfNotExist(userDetails, GlobalUserSource.Microsoft);
                notification.HandleCodeRedemption(null, result.IdToken);
                
            }
            catch (MsalException ex)
            {
                string message = "AcquireTokenByAuthorizationCodeAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/User/Error?message={message}&debug={ex.Message}");
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                string message = "GetUserDetailsAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/User/Error?message={message}&debug={ex.Message}");
            }
        }


        private void AddUserIfNotExist(CachedUser user, GlobalUserSource source)
        {
            var userDAL = DependencyResolver.Current.GetService<IGlobalUserDAL>();

            if (!userDAL.ExistByEmailAndSource(user.Email, source)) 
            {
                userDAL.InsertOrUpdate(new GlobalThesaurusUser()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Source = source,
                    EntryDatetime = DateTime.Now
                }) ;
            }
        }
    }
}
