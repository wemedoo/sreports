using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.User.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IGlobalUserBLL
    {
        UserDataOut TryLoginUser(UserLoginDataIn user);
    }
}
