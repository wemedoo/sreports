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
        /*[HttpPut]
        [SReportsAutorize]
        public ActionResult UpdateLanguage([System.Web.Http.FromBody] EnumDTO data)
        {
            data = Ensure.IsNotNull(data, nameof(data));

            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userService.UpdateLanguage(userCookieData.Username, data.Value);
                userCookieData.ActiveLanguage = data.Value;
                Session["userData"] = userCookieData;
                ChangeLanguage(userCookieData);
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
                this.userService.UpdatePageSize(userCookieData.Username, data.PageSize);
                userCookieData.PageSize = data.PageSize;
                Session["userData"] = userCookieData;
                ChangeLanguage(userCookieData);
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
        public ActionResult UpdateOrganization([System.Web.Http.FromBody] EnumDTO data)
        {
            data = Ensure.IsNotNull(data, nameof(data));

            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userService.UpdateOrganization(userCookieData.Username, data.Value);
                userCookieData.ActiveOrganization = data.Value;
                Session["userData"] = userCookieData;
                ResetCookie(userCookieData.Username, data.Value);
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
        }

        public void ChangeLanguage(UserCookieData userCookieData)
        {
            userCookieData = Ensure.IsNotNull(userCookieData, nameof(userCookieData));

            if (userCookieData.ActiveLanguage != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(userCookieData.ActiveLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(userCookieData.ActiveLanguage);
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = userCookieData.ActiveLanguage;
            Response.Cookies.Add(cookie);
        }

        public void ResetCookie(string username, string organizationId) 
        {
            List<string> newRoles = userService.GetByUsername(username).GetRolesByActiveOrganization(organizationId);
            var authTicket = new FormsAuthenticationTicket(
                    1,                             // version
                    userCookieData.Username,                // user name
                    DateTime.Now,                  // created
                    DateTime.Now.AddMinutes(20),   // expires
                    true,                    // persistent?
                    newRoles != null ? string.Join(";", newRoles) : string.Empty                      // can be used to store roles
                    );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
        }*/
    }
}