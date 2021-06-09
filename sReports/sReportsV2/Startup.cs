using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;

[assembly: OwinStartup(typeof(sReportsV2.Startup))]

namespace sReportsV2
{
    public partial class Startup
    {
        // Load configuration settings from PrivateSettings.config
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
