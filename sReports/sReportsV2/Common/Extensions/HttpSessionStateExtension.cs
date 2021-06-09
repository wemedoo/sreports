using AutoMapper;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
                

                //TO DO PULL ORGANIZATIONS
                var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
                User userEntity = userDAL.GetByUsername(ticketDecrypted.Name);
                userCookieData = Mapper.Map<UserCookieData>(userEntity);
                userCookieData.Organizations = null;
                sessionState["userData"] = userCookieData;
            }

            return userCookieData;
        }
    }
}