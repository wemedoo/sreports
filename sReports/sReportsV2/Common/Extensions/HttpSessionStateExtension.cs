using AutoMapper;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Organization;
using sReportsV2.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace sReportsV2.Common.Extensions
{
    public static class HttpSessionStateExtension
    {
        public static UserCookieData GetUserFromSession(this HttpSessionState sessionState)
        {
            Ensure.IsNotNull(sessionState, nameof(sessionState));

            UserCookieData userCookieData = sessionState["userData"] as UserCookieData;
            if (userCookieData == null)
            {
                var ticket = System.Web.HttpContext.Current.Response.Cookies.Get(FormsAuthentication.FormsCookieName);
                var ticketDecrypted = FormsAuthentication.Decrypt(ticket.Value);
                

                UserService userService = new UserService();
                OrganizationService organizationService = new OrganizationService();

                User userEntity = userService.GetByUsername(ticketDecrypted.Name);
                userCookieData = Mapper.Map<UserCookieData>(userEntity);
                userCookieData.Organizations = Mapper.Map<List<OrganizationDataOut>>(organizationService.GetOrganizationsByIds(userEntity.OrganizationRefs));
                sessionState["userData"] = userCookieData;
            }

            return userCookieData;
        }
    }
}