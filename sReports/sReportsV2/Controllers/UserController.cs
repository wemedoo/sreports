using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.EmailHelper;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.Models.User;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace sReportsV2.Controllers
{
    public class UserController : BaseController
    {

        private readonly IUserService userService;
        private readonly IOrganizationService organizationService;
        public UserController()
        {
            this.userService = new UserService();
            this.organizationService = new OrganizationService();
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(Uri ReturnUrl)
        {
            ReturnUrl = Ensure.IsNotNull(ReturnUrl, nameof(ReturnUrl));

            ViewBag.ReturnUrl = ReturnUrl.ToString() != "/User/Logout" ? ReturnUrl.ToString() : "/";
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserModelView userDataIn)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            System.Web.Helpers.AntiForgery.Validate();

            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                if (this.userService.IsValidUser(userDataIn.Username, userDataIn.Password))
                {
                    User userEntity = this.userService.GetByUsername(userDataIn.Username);
                    UserCookieData userCookieData = Mapper.Map<UserCookieData>(userEntity);
                    userCookieData.Organizations = Mapper.Map<List<OrganizationDataOut>>(this.organizationService.GetOrganizationsByIds(userEntity.OrganizationRefs));
                    //FormsAuthentication.SetAuthCookie(userCookieData.Username, false);
                    var authTicket = new FormsAuthenticationTicket(
                    1,                             // version
                    userCookieData.Username,                      // user name
                    DateTime.Now,                  // created
                    DateTime.Now.AddMinutes(20),   // expires
                    true,                    // persistent?
                    userEntity.Roles != null ? string.Join(";", userEntity.Roles) : string.Empty                      // can be used to store roles
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                    ChangeLanguage(userCookieData);
                    if (!string.IsNullOrEmpty(userDataIn.ReturnUrl.ToString()))
                    {
                        return Redirect(userDataIn.ReturnUrl.ToString());
                    }
                    return RedirectToAction("GetAll", "EpisodeOfCare");
                }
            }
            ModelState.AddModelError("General", "Invalid Username or Password");
            return View(userDataIn);
        }

        [HttpPut]
        [Authorize]
        public ActionResult UpdateLanguage([System.Web.Http.FromBody]EnumDTO data)
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
            catch(Exception ex) 
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPut]
        [Authorize]
        public ActionResult UpdatePageSizeSettings([System.Web.Http.FromBody]UserUpdatePageSizeDataIn data)
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
        [Authorize]
        public ActionResult UpdateOrganization([System.Web.Http.FromBody]EnumDTO data)
        {
            data = Ensure.IsNotNull(data, nameof(data));

            try
            {
                UserCookieData userCookieData = System.Web.HttpContext.Current.Session.GetUserFromSession();
                this.userService.UpdateOrganization(userCookieData.Username, data.Value);
                userCookieData.ActiveOrganization = data.Value;
                Session["userData"] = userCookieData;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            FormsAuthentication.RedirectToLoginPage();
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

        [Authorize]
        public ActionResult GetAll(DataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [Authorize]
        public ActionResult ReloadTable(DataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            PaginationDataOut<UserDataDataOut, DataIn> result = new PaginationDataOut<UserDataDataOut, DataIn>()
            {
                Count = (int)this.userService.GetAllEntriesCount(),
                Data = Mapper.Map<List<UserDataDataOut>>(this.userService.GetAll(dataIn.PageSize, dataIn.Page)),
                DataIn = dataIn
            };
            for (int i = 0; i < result.Data.Count; i++)
            {
                List<OrganizationDataOut> organizations = new List<OrganizationDataOut>();
                for (int j = 0; j < result.Data[i].Organizations.Count; j++)
                {
                    OrganizationDataOut organization = Mapper.Map<OrganizationDataOut>(this.organizationService.GetOrganizationById(result.Data[i].Organizations[j].Id));
                    organizations.Add(organization);
                }
                result.Data[i].Organizations = organizations;
            }

            return PartialView("UserEntryTable", result);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Organizations = organizationService.GetOrganizations();
            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return View("User");
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(UserDataDataIn user)
        {
            user = Ensure.IsNotNull(user, nameof(user));

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string activeOrganization = SetActiveOrganization(user);
            string activeLanguage = userCookieData.ActiveLanguage;

            try
            {
                userService.Insert(Mapper.Map<User>(user), activeOrganization, activeLanguage);
                if (user.Id == null)
                {
                    User userFromDB = userService.GetByUsername(user.Username);
                    Task.Run(() => EmailSender.SendAsync("sReports password", "Your password is: " + userFromDB.Password + ". Please change your password on your first login. ", "", userFromDB.Email));
                }
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [Authorize]
        public ActionResult Edit(string userId)
        {
            User user = userService.GetUserById(userId);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            UserDataDataOut result = Mapper.Map<UserDataDataOut>(user);

            ViewBag.Organizations = organizationService.GetOrganizations();
            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return View("User", result);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string userId, DateTime lastUpdate)
        {
            try
            {
                this.userService.Delete(userId, lastUpdate);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDeleteEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }
            catch (MongoDbConcurrencyDeleteException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDelete;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize]
        public ActionResult UserProfile(string userId)
        {
            User user = userService.GetUserById(userId);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            UserDataDataOut result = Mapper.Map<UserDataDataOut>(user);

            ViewBag.Organizations = organizationService.GetOrganizations();
            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return View("User", result);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            oldPassword = Ensure.IsNotNull(oldPassword, nameof(oldPassword));
            newPassword = Ensure.IsNotNull(newPassword, nameof(newPassword));
            confirmPassword = Ensure.IsNotNull(confirmPassword, nameof(confirmPassword));

            User user = userService.GetUserById(userId);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            
            if(!oldPassword.Equals(user.Password))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!newPassword.Equals(confirmPassword))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            try
            {
                userService.UpdatePassword(user, newPassword);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        private string SetActiveOrganization(UserDataDataIn user)
        {
            string activeOrganization = string.Empty;
            if (user.Organizations != null && user.Organizations.Count != 0)
                activeOrganization = user.Organizations[0];
            else
                activeOrganization = userCookieData.ActiveOrganization;

            return activeOrganization;
        }

    }
}