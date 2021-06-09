using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class GlobalUserBLL : IGlobalUserBLL
    {
        private readonly HttpContextBase context;
        IGlobalUserDAL globalUserDAL;
        public GlobalUserBLL(IGlobalUserDAL globalUserDAL, HttpContextBase context)
        {
            this.globalUserDAL = globalUserDAL;
            this.context = context;
        }
        public UserDataOut TryLoginUser(UserLoginDataIn user)
        {
            UserDataOut result = null;
            if (globalUserDAL.IsValidUser(user.Username, user.Password))
            {
                GlobalThesaurusUser globalUser = this.globalUserDAL.GetByEmail(user.Username);
                UserCookieData userCookieData = Mapper.Map<UserCookieData>(globalUser);
                
            }

            return result;
        }
    }
}
