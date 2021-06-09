using AutoMapper;
using Microsoft.Owin.Security.Cookies;
using Serilog;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.Models.MicrosoftAuthentification;

using sReportsV2.TokenStorage;
using System.Collections.Generic;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;

namespace sReportsV2.Controllers
{
    public class BaseController : Controller
    {
        protected UserCookieData userCookieData;
        //private IOrganizationService organizationService;
        public BaseController()
        {
            //discuss this with Danilo
            var emailClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).FindFirst(ClaimTypes.Email);
            emailClaim = emailClaim != null ? emailClaim : ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).FindFirst("preferred_username");
            string email = emailClaim?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                userCookieData = System.Web.HttpContext.Current.Session["userData"] as UserCookieData;
                if (userCookieData == null)
                {
                    if (ConfigurationManager.AppSettings["Instance"] == InstanceNames.SReports)
                    {
                        SetUserCookieDataForSReports(email);
                    }
                    else if (ConfigurationManager.AppSettings["Instance"] == InstanceNames.ThesaurusGlobal) 
                    {
                        SetUserCookieDataForThesaurusGlobal(email);
                    }
                }
                ViewBag.UserCookieData = userCookieData;
            }
            ViewBag.Languages = SingletonDataContainer.Instance.GetLanguages();
            ViewBag.Env = ConfigurationManager.AppSettings["Environment"];
            SetLocalDateFormat();
        }

        public ActionResult NotFound(string resourceId)
        {
            Log.Warning(SReportsResource.FormNotExists, 404, resourceId);
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        private void SetUserCookieDataForSReports(string email) 
        {
            var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
            User userEntity = userDAL.GetByEmail(email);
            userCookieData = Mapper.Map<UserCookieData>(userEntity);
            System.Web.HttpContext.Current.Session["userData"] = userCookieData;

        }

        private void SetUserCookieDataForThesaurusGlobal(string email)
        {
            var globalUserDAL = DependencyResolver.Current.GetService<IGlobalUserDAL>();
            GlobalThesaurusUser userEntity = globalUserDAL.GetByEmail(email);
            userCookieData = Mapper.Map<UserCookieData>(userEntity);
            userCookieData.ActiveLanguage = "en";
            System.Web.HttpContext.Current.Session["userData"] = userCookieData;

        }

        private void SetLocalDateFormat()
        {
            if (userCookieData != null)
            {
                CultureInfo cultureInfo = new CultureInfo(userCookieData.ActiveLanguage);
                if (cultureInfo.DateTimeFormat.ShortDatePattern.Contains("yyyy"))
                    ViewBag.DateFormat = cultureInfo.DateTimeFormat.ShortDatePattern.Replace("yyyy", "yy");
                else
                    ViewBag.DateFormat = cultureInfo.DateTimeFormat.ShortDatePattern;
            }
        }

        protected void Flash(string message, string debug = null)
        {
            var alerts = TempData.ContainsKey(Alert.AlertKey) ?
                (List<Alert>)TempData[Alert.AlertKey] :
                new List<Alert>();

            alerts.Add(new Alert
            {
                Message = message,
                Debug = debug
            });

            TempData[Alert.AlertKey] = alerts;
        }
    }
}
