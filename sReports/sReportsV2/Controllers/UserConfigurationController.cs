using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace sReportsV2.Controllers
{
    public partial class UserController
    {
        [HttpPut]
        [SReportsAutorize]
        public ActionResult UpdateLanguage([System.Web.Http.FromBody] EnumDTO data)
        {
            data = Ensure.IsNotNull(data, nameof(data));

            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userBLL.UpdateLanguage(data, userCookieData);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [SReportsAutorize]
        public ActionResult UpdatePageSizeSettings([System.Web.Http.FromBody] UserUpdatePageSizeDataIn data)
        {
            data = Ensure.IsNotNull(data, nameof(data));

            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userBLL.UpdatePageSize(data.PageSize, userCookieData);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAutorize]
        public ActionResult UpdateOrganization(int organizationId)
        {
            UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();

            userBLL.SetActiveOrganization(userCookieData, organizationId);
            //ResetCookie(userCookieData.Username, data.Value);
            //TO DO IMPORTANT: if we update roles to user organization level, we'll have to reset data in the context to update roles for the active org
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /*
        [HttpPut]
        [SReportsAutorize]
        public ActionResult AddSuggestedForm([System.Web.Http.FromBody] string formId)
        {
            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                userCookieData.SuggestedForms.Add(formId);
                this.userService.AddSugestedForm(userCookieData.Username, formId);
                System.Web.HttpContext.Current.Session["userData"] = userCookieData;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [SReportsAutorize]
        public ActionResult RemoveSuggestedForm([System.Web.Http.FromBody] string formId)
        {
            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userService.RemoveSugestedForm(userCookieData.Username, formId);
                userCookieData.SuggestedForms.RemoveAt(userCookieData.SuggestedForms.IndexOf(formId));
                System.Web.HttpContext.Current.Session["userData"] = userCookieData;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }*/
    }
}