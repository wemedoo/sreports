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
        [SReportsAuthorize]
        public ActionResult UpdateLanguage([System.Web.Http.FromBody] EnumDTO data)
        {
            data = Ensure.IsNotNull(data, nameof(data));
            UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
            this.userBLL.UpdateLanguage(data, userCookieData);

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [SReportsAuthorize]
        public ActionResult UpdatePageSizeSettings([System.Web.Http.FromBody] UserUpdatePageSizeDataIn data)
        {
            data = Ensure.IsNotNull(data, nameof(data));
            UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
            this.userBLL.UpdatePageSize(data.PageSize, userCookieData);

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [SReportsAuthorize]
        public ActionResult AddSuggestedForm([System.Web.Http.FromBody] string formId)
        {
            UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
            userCookieData.SuggestedForms.Add(formId);
            this.userBLL.AddSuggestedForm(userCookieData.Username, formId);
            System.Web.HttpContext.Current.Session["userData"] = userCookieData;

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [SReportsAuthorize]
        public ActionResult RemoveSuggestedForm([System.Web.Http.FromBody] string formId)
        {
            UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
            this.userBLL.RemoveSuggestedForm(userCookieData.Username, formId);
            userCookieData.SuggestedForms.RemoveAt(userCookieData.SuggestedForms.IndexOf(formId));
            System.Web.HttpContext.Current.Session["userData"] = userCookieData;

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
    }
}