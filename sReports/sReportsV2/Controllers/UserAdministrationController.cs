using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class UserAdministrationController : BaseController
    {
        private readonly IFormDAL formService;
        private readonly IUserBLL userBLL;
        private readonly IRoleBLL roleBLL;

        public UserAdministrationController(IUserBLL userBLL, IRoleBLL roleBLL)
        {
            formService = new FormDAL();
            this.userBLL = userBLL;
            this.roleBLL = roleBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll(UserFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(UserFilterDataIn dataIn)
        {
            var result = userBLL.ReloadTable(dataIn, userCookieData.ActiveOrganization);
            return PartialView("UserEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        public ActionResult Create()
        {
            return GetUser(isUserAdministration: true, shouldRetrieveUser:  false);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsUserValidate]
        public ActionResult Create(UserDataIn user)
        {
            CreateResponseResult response = userBLL.Insert(user, userCookieData.ActiveLanguage);
            response.Message = user.Id == 0 ? Resources.TextLanguage.UserAdministrationMsgCreate : Resources.TextLanguage.UserAdministrationMsgEdit;
            UpdateUserCookieIfNecessary(response.Id == userCookieData.Id, user.Email);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateUserOrganizations(UserDataIn userDataIn)
        {
            CreateResponseResult response = userBLL.UpdateOrganizations(userDataIn);
            response.Message = Resources.TextLanguage.UserAdministrationMsgEdit;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateUserClinicalTrials(UserDataIn userDataIn)
        {
            CreateResponseResult response = userBLL.UpdateClinicalTrials(userDataIn);
            response.Message = Resources.TextLanguage.UserAdministrationMsgEdit;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult Edit(int userId)
        {
            return GetUser(isUserAdministration : true, userId: userId);
        }

        [SReportsAuthorize]
        [HttpPost]
        public ActionResult LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            if (dataIn.OrganizationsIds != null && dataIn.OrganizationsIds.Contains(dataIn.OrganizationId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Resources.TextLanguage.OrganizationExist);
            }

            var result = userBLL.LinkOrganization(dataIn);
            ViewBag.UserAdministration = true;
            return PartialView("OrganizationData", result);
        }

        
        [SReportsAuthorize]
        public ActionResult ResetClinicalTrial(int? clinicalTrialId)
        {
            ViewBag.UserAdministration = true;
            return PartialView("UserClinicalTrial", userBLL.ResetClinicalTrial(clinicalTrialId));
        } 

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult SubmitClinicalTrial(ClinicalTrialWithUserInfoDataIn dataIn)
        {
            ViewBag.Message = Resources.TextLanguage.UserAdministrationMsgEdit;
            ViewBag.UserAdministration = true;
            return PartialView("ClinicalTrialLists", userBLL.SubmitClinicalTrial(dataIn));
        }



        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult ArchiveClinicalTrial(ArchiveTrialDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            byte[] RowVersion = userBLL.ArchiveClinicalTrial(dataIn);
            
            return Json(new CreateResponseResult()
            {
                Id = dataIn.UserId,
                RowVersion = RowVersion,
                Message = Resources.TextLanguage.UserAdministrationMsgEdit
            }, JsonRequestBehavior.AllowGet);
            
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        public ActionResult SetUserState(int userId, UserState? newState)
        {
            if (userBLL.UserExist(userId))
            {
                userBLL.SetState(userId, newState, userCookieData.ActiveOrganization);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        

        [SReportsAuthorize]
        public ActionResult UserProfile(int userId)
        {
            return GetUser(isUserAdministration: false, userId: userId);
        }

        [SReportsAuthorize]
        public ActionResult DisplayUser(int userId)
        {
            SetReadOnlyAndDisabledViewBag(true);
            return GetUser(isUserAdministration: false, userId: userId);
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsUserValidate]
        public ActionResult UpdateUserProfile(UserDataIn user)
        {
            CreateResponseResult response = userBLL.Insert(user, userCookieData.ActiveLanguage);
            UpdateUserCookieIfNecessary(true, user.Email);
            response.Message = user.Id == 0 ? Resources.TextLanguage.UserAdministrationMsgCreate : Resources.TextLanguage.UserAdministrationMsgEdit;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            userBLL.ChangePassword(oldPassword, newPassword, confirmPassword, userId);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }


        [SReportsAuthorize]
        public ActionResult CheckUsername(string username, string currentUsername)
        {
            if (username == currentUsername) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            bool isValid = userBLL.IsUsernameValid(username);
            return isValid ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [SReportsAuthorize]
        public ActionResult CheckEmail(string email, string currentEmail)
        {
            if (email == currentEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            bool isValid = userBLL.IsEmailValid(email);
            return isValid ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult GetByName(string searchValue)
        {
            searchValue = searchValue.RemoveDiacritics(); // Normalization

            List<UserDataOut> users = this.userBLL.GetUsersByName(searchValue);
            return PartialView("~/Views/User/GetByName.cshtml", users.OrderBy(x => x.FirstName).ToList());
        }

        [HttpPut]
        [Authorize]
        public ActionResult UpdateOrganization(int organizationId)
        {
            userBLL.SetActiveOrganization(userCookieData, organizationId);
            //ResetCookie(userCookieData.Username, data.Value);
            //TO DO IMPORTANT: if we update roles to user organization level, we'll have to reset data in the context to update roles for the active org
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void UpdateUserCookieIfNecessary(bool needsToBeUpdated, string email)
        {
            if(needsToBeUpdated)
            {
                UpdateUserCookie(email);
                UpdateClaims(new Dictionary<string, string>() { {ClaimTypes.Email, email}, { "preferred_username", email} });
            }
        }

        private ActionResult GetUser(bool isUserAdministration, int userId = 0, bool shouldRetrieveUser = true)
        {
            UserDataOut viewModel = null;
            if (shouldRetrieveUser)
            {
                viewModel = userBLL.GetUserForEdit(userId);
            }

            ViewBag.Roles = roleBLL.GetAll();
            ViewBag.UserAdministration = isUserAdministration;
            
            return View("User", viewModel);
        }
    }
}