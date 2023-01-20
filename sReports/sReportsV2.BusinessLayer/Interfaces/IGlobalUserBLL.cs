using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IGlobalUserBLL
    {
        GlobalThesaurusUserDataOut GetByEmail(string email);
        bool ExistByEmailAndSource(string email, GlobalUserSource source);
        GlobalThesaurusUserDataOut InsertOrUpdate(GlobalThesaurusUserDataIn user);
        GlobalThesaurusUserDataOut TryLoginUser(string username, string password);
        void SubmitContactForm(ContactFormDataIn contactFormData);
        List<GlobalThesaurusUserDataOut> GetUsers();
        void ActivateUser(string email);
        void SetUserStatus(int id, GlobalUserStatus status);
        List<RoleDataOut> GetRoles();
        void UpdateRoles(GlobalThesaurusUserDataIn user);
        bool IsReCaptchaInputValid(string response, string secretKey);
    }
}
