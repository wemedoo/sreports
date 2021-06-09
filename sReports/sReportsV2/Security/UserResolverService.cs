using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Security
{
    public static class UserResolverService
    {
        public static UserCookieData GetUser()
        {
            return new UserCookieData(System.Web.HttpContext.Current.User.Identity.Name);
        }
    }
}