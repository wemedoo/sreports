using AutoMapper;
using Newtonsoft.Json.Linq;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class GlobalUserBLL : IGlobalUserBLL
    {
        private readonly IGlobalThesaurusUserDAL globalUserDAL;
        private readonly IGlobalThesaurusRoleDAL globalThesaurusRoleDAL;
        private readonly IEmailSender emailSender;

        public GlobalUserBLL(IGlobalThesaurusUserDAL globalUserDAL, IGlobalThesaurusRoleDAL globalThesaurusRoleDAL, IEmailSender emailSender)
        {
            this.globalUserDAL = globalUserDAL;
            this.globalThesaurusRoleDAL = globalThesaurusRoleDAL;
            this.emailSender = emailSender;
        }

        public void ActivateUser(string email)
        {
            var user = globalUserDAL.GetByEmail(email);
            if(user != null && user.Status == GlobalUserStatus.NotVerified)
            {
                user.Status = GlobalUserStatus.Active;
                globalUserDAL.InsertOrUpdate(user);
            }
        }

        public bool ExistByEmailAndSource(string email, GlobalUserSource source)
        {
            return globalUserDAL.ExistByEmailAndSource(email, source);
        }

        public GlobalThesaurusUserDataOut GetByEmail(string email)
        {
            return Mapper.Map<GlobalThesaurusUserDataOut>(globalUserDAL.GetByEmail(email));
        }

        public List<RoleDataOut> GetRoles()
        {
            return Mapper.Map<List<RoleDataOut>>(globalThesaurusRoleDAL.GetAll());
        }

        public List<GlobalThesaurusUserDataOut> GetUsers()
        {
            List<GlobalThesaurusUser> filteredUsers = new List<GlobalThesaurusUser>();
            foreach (var user in globalUserDAL.GetAll())
            {
                if (!user.HasRole(SmartOncologyRoleNames.SuperAdministrator))
                {
                    filteredUsers.Add(user);
                }
            }
            return Mapper.Map<List<GlobalThesaurusUserDataOut>>(filteredUsers);
        }

        public GlobalThesaurusUserDataOut InsertOrUpdate(GlobalThesaurusUserDataIn user)
        {
            GlobalThesaurusUser userDb = Mapper.Map<GlobalThesaurusUser>(user);
            if (userDb.GlobalThesaurusUserId == 0)
            {
                SetPredifinedRole(userDb, PredifinedGlobalUserRole.Viewer);
                SendActivationLink(userDb);
            }
            return Mapper.Map<GlobalThesaurusUserDataOut>(globalUserDAL.InsertOrUpdate(userDb));
        }

        public bool IsReCaptchaInputValid(string response, string secretKey)
        {
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            bool reCaptchaInputSuccess = (bool)obj.SelectToken("success");
            var score = obj.SelectToken("score");
            double reCaptchaInputScore = score != null ? (double)score : 0;

            return reCaptchaInputSuccess && reCaptchaInputScore > 0.5;
        }

        public void SetUserStatus(int id, GlobalUserStatus status)
        {
            var user = globalUserDAL.GetById(id);
            if (user != null)
            {
                user.Status = status;
                globalUserDAL.InsertOrUpdate(user);
            }
        }

        public void SubmitContactForm(ContactFormDataIn contactFormData)
        {
            string mailContentIntro = $"Dear {contactFormData.FullName},<br><br>We recieved your contact form. Here is the content of your inquiry";
            string mailContentBody = $"<table border=\"1\" style=\"width: 25%;\">" +
                $"<tr><th>Full Name</th><td>{contactFormData.FullName}</td></tr>" +
                $"<tr><th>Email</th><td>{contactFormData.Email}</td></tr>" +
                $"<tr><th>Role</th><td>{contactFormData.Role}</td></tr>" +
                $"<tr><th>Organization</th><td>{contactFormData.Organization}</td></tr>" +
                $"<tr><th>Adress</th><td>{contactFormData.Address}</td></tr>" +
                $"<tr><th>Phone</th><td>{contactFormData.Phone}</td></tr>" +
                $"<tr><th>Message</th><td>{contactFormData.Message}</td></tr></table>";
            string mailContent = $"<div>" +
                $"{mailContentIntro}<br>" + $"<br>{mailContentBody}<br><br>" +
                $"------------------------------------------------------------------------------" +
                $"</div>";
            string hostEmail = WebConfigurationManager.AppSettings["AppEmail"];

            Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} Contact Form", string.Empty, mailContent, contactFormData.Email));
            Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} Contact Form", string.Empty, mailContent, hostEmail));
        }

        public GlobalThesaurusUserDataOut TryLoginUser(string username, string password)
        {
            GlobalThesaurusUser user = null;
            if(globalUserDAL.IsValidUser(username, password))
            {
                user = globalUserDAL.GetByEmail(username);
            }
            return Mapper.Map<GlobalThesaurusUserDataOut>(user);
        }

        public void UpdateRoles(GlobalThesaurusUserDataIn user)
        {
            GlobalThesaurusUser userDb = globalUserDAL.GetById(user.Id);
            userDb.UpdateRoles(user.Roles);
            globalUserDAL.InsertOrUpdate(userDb);
        }

        private void SendActivationLink(GlobalThesaurusUser user)
        {
            string mailContent = $"<div>" +
                    $"Dear {user.FirstName} {user.LastName},<br><br>you are granted access to the Smart Oncology clinical information system.<br>" +
                    $"Use the following link to activate your account in the system: <a href='{HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)}/ThesaurusGlobal/ActivateUser?email={user.Email}'>Activation link</a><br>" +
                    $"Your username is your email: <b>{user.Email}</b><br>" +
                    $"Your password is: <b>{user.Password}</b><br><br>" +
                    $"If you need any help or you have any additional questions please write on the <a href='mailto:smartoncology@wemedoo.com'>smartoncology@wemedoo.com</a>.<br><br>" +
                    $"------------------------------------------------------------------------------" +
                    $"</div>";
            Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} Registration", string.Empty, mailContent, user.Email));
        }

        private void SetPredifinedRole(GlobalThesaurusUser user, PredifinedGlobalUserRole predifinedGlobalUserRole)
        {
            GlobalThesaurusRole role = globalThesaurusRoleDAL.GetByName(predifinedGlobalUserRole.ToString());
            user.UpdateRoles(new List<int>() { role.GlobalThesaurusRoleId });
        }
    }
}
