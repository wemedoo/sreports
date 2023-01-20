using AutoMapper;
using Microsoft.Owin.Security.Cookies;
using Serilog;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Singleton;
using sReportsV2.Models.MicrosoftAuthentification;
using sReportsV2.TokenStorage;
using System.Collections.Generic;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Infrastructure;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using Microsoft.Owin.Security;

namespace sReportsV2.Controllers
{
    public class BaseController : Controller
    {
        protected UserCookieData userCookieData;
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
            ViewBag.PatientLanguages = SingletonDataContainer.Instance.GetPatientLanguages();
            ViewBag.Env = ConfigurationManager.AppSettings["Environment"];
            SetLocalDateFormat();
        }

        public ActionResult NotFound(string resourceId)
        {
            Log.Warning(SReportsResource.FormNotExists, 404, resourceId);
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
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

        protected void UpdateUserCookie(string email)
        {
            SetUserCookieDataForSReports(email);
            ViewBag.UserCookieData = userCookieData;
        }

        protected void SignOut()
        {
            var tokenStore = new SessionTokenStore(null,
                      System.Web.HttpContext.Current, ClaimsPrincipal.Current);

            tokenStore.Clear();
            System.Web.HttpContext.Current.Session["userData"] = null;
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType, DefaultAuthenticationTypes.ApplicationCookie);
        }

        protected void SetCustomResponseHeaderForMultiFileDownload()
        {
            Response.Headers.Add("MultiFile", "true");
            string lastFile = Request.Headers.Get("LastFile");
            Response.Headers.Add("LastFile", string.IsNullOrWhiteSpace(lastFile) ? "true" : lastFile);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));
            HandleException(filterContext);
        }

        protected void UpdateClaims(Dictionary<string, string> claims)
        {
            var claimsIdentity = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity);
            if (claimsIdentity == null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> keyValue in claims)
            {
                var existingClaim = claimsIdentity.FindFirst(keyValue.Key);
                if (existingClaim != null)
                {
                    claimsIdentity.RemoveClaim(existingClaim);
                }

                claimsIdentity.AddClaim(new Claim(keyValue.Key, keyValue.Value));
            }

            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { IsPersistent = false });
        }

        protected void SetReadOnlyAndDisabledViewBag(bool readOnly)
        {
            ViewBag.ReadOnly = readOnly;
            ViewBag.Disabled = readOnly ? "disabled" : "";
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
            var globalUserDAL = DependencyResolver.Current.GetService<IGlobalThesaurusUserDAL>();
            GlobalThesaurusUser userEntity = globalUserDAL.GetByEmail(email);
            userCookieData = Mapper.Map<UserCookieData>(userEntity);
            userCookieData.ActiveLanguage = "en";
            System.Web.HttpContext.Current.Session["userData"] = userCookieData;
        }

        private void SetLocalDateFormat()
        {
            if (userCookieData != null)
            {
                ViewBag.DateFormatClient = DateConstants.DateFormatClient;
                ViewBag.DateFormatDisplay = DateConstants.DateFormatDisplay;
                ViewBag.TimeFormatDisplay = DateConstants.TimeFormatDisplay;
                ViewBag.DateFormat = DateConstants.DateFormat;
            }
        }

        private void HandleException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            string exTypeName = ex.GetType()?.Name;
            switch (ex)
            {
                case ConcurrencyException _:
                case DbUpdateConcurrencyException _:
                    HandleException(filterContext, HttpStatusCode.Conflict, Resources.TextLanguage.ConcurrencyExEdit, exTypeName);
                    break;
                case ConcurrencyDeleteException _:
                    HandleException(filterContext, HttpStatusCode.Conflict, Resources.TextLanguage.ConcurrencyExDelete, exTypeName);
                    break;
                case ConcurrencyDeleteEditException _:
                    HandleException(filterContext, HttpStatusCode.Conflict, Resources.TextLanguage.ConcurrencyExDeleteEdit, exTypeName);
                    break;
                case NullReferenceException _:
                    var responseMessage = ex.Message.Equals("Object reference not set to an instance of an object.'") ? Resources.TextLanguage.NotFound : ex.Message;
                    HandleException(filterContext, HttpStatusCode.NotFound, responseMessage, exTypeName);
                    break;
                case IterationNotFinishedException _:
                    HandleException(filterContext, HttpStatusCode.BadRequest, Resources.TextLanguage.Current_Iteration_is_not_finished, exTypeName);
                    break;
                case ConsensusCannotStartException consensusCannotStartEx:
                    string errorMessage = Resources.TextLanguage.Cannot_start_CF + consensusCannotStartEx.FormItemLevel;
                    HandleException(filterContext, HttpStatusCode.BadRequest, errorMessage, exTypeName);
                    break;
                case UserAdministrationException userAdministrationEx:
                    HandleException(filterContext, userAdministrationEx.HttpStatusCode, ex.Message, exTypeName);
                    break;
                case DuplicateException _:
                    HandleException(filterContext, HttpStatusCode.Conflict, ex.Message, exTypeName);
                    break;
                case ApiCallException _:
                    HandleException(filterContext, HttpStatusCode.InternalServerError, "Error calling external api service.", exTypeName);
                    break;
                default:
                    string unknownExceptionMsg = "Unkown exception! Please contact your administrator for more details.";
                    HandleException(filterContext, HttpStatusCode.InternalServerError, unknownExceptionMsg, $"Unkown -> {exTypeName}");
                    break;
            }
        }

        private void HandleException(ExceptionContext exceptionContext, HttpStatusCode errorStatusCode, string responseErrorMessage, string exType)
        {
            if (Request.IsAjaxRequest())
            {
                exceptionContext.Result = new HttpStatusCodeResult(errorStatusCode, responseErrorMessage);
            }
            else
            {
                exceptionContext.Result = new ViewResult { ViewName = "Error", ViewData = new ViewDataDictionary(ViewData) { { "ErrorMessage", responseErrorMessage } } };
            }
            exceptionContext.ExceptionHandled = true;
            LogException(exceptionContext.Exception, exType);
        }

        private void LogException(Exception exception, string exType)
        {

            Log.Error($"<--- Exception [{exType}]: is thrown in ({Request.HttpMethod} {Request.RawUrl}) --->");
            Log.Error(exception.Message);
            Log.Error(exception.StackTrace);
        }
    }
}
