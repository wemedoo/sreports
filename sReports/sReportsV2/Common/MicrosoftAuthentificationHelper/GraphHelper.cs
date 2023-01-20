using Microsoft.Graph;
using sReportsV2.Models.MicrosoftAuthentification;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sReportsV2.Common.MicrosoftAuthentificationHelper
{
    public class GraphHelper
    {
        public static async Task<CachedUser> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            var user = await graphClient.Me.Request()
                .Select(u => new {
                    u.Mail,
                    u.UserPrincipalName,
                    u.GivenName,
                    u.Surname
                })
                .GetAsync();

            return new CachedUser
            {
                FirstName = user.GivenName,
                LastName = user.Surname,
                Email = string.IsNullOrEmpty(user.Mail) ?
                    user.UserPrincipalName : user.Mail
            };
        }
    }
}