using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class UserAdministrationController : BaseController
    {
        private readonly IFormDAL formService;
        private readonly IUserBLL userBLL;
        public UserAdministrationController(IUserBLL userBLL)
        {
            formService = new FormDAL();
            this.userBLL = userBLL;
        }

        [SReportsAutorize]
        public ActionResult GetAll(DataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(DataIn dataIn)
        {
            var result = userBLL.ReloadTable(dataIn, userCookieData.ActiveOrganization);
            return PartialView("UserEntryTable", result);
        }

        [SReportsAutorize]
        public ActionResult Create()
        {
            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return View("User");
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsUserValidate]
        public ActionResult Create(UserDataIn user)
        {
            var response = userBLL.Insert(user, userCookieData.ActiveLanguage);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateUserOrganizations(UserDataIn userDataIn)
        {
            return Json(userBLL.UpdateOrganizations(userDataIn), JsonRequestBehavior.AllowGet);
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult UpdateUserClinicalTrials(UserDataIn userDataIn)
        {
            return Json(userBLL.UpdateClinicalTrials(userDataIn), JsonRequestBehavior.AllowGet);
        }

        [SReportsAutorize]
        public ActionResult Edit(int userId)
        {            
            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();           
            return View("User", userBLL.GetUserForEdit(userId));
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            var result = userBLL.LinkOrganization(dataIn);

            if (dataIn.OrganizationsIds != null && dataIn.OrganizationsIds.Contains(result.Organization.Id)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Resources.TextLanguage.OrganizationExist);
            }

            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return PartialView("OrganizationData", result);
        }

        
        [SReportsAutorize]
        public ActionResult ResetClinicalTrial(int? clinicalTrialId)
        {            
            return PartialView("UserClinicalTrial", userBLL.ResetClinicalTrial(clinicalTrialId));
        } 

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult SubmitClinicalTrial(ClinicalTrialWithUserInfoDataIn dataIn)
        {
            return PartialView("ClinicalTrialLists", userBLL.SubmitClinicalTrial(dataIn));
        }



        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult ArchiveClinicalTrial(ArchiveTrialDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            byte[] RowVersion = userBLL.ArchiveClinicalTrial(dataIn);
            
            return Json(new CreateResponseResult()
            {
                Id = dataIn.UserId,
                RowVersion = RowVersion
            }, JsonRequestBehavior.AllowGet);
            
        }

        
        [SReportsAutorize]
        public ActionResult SetUserState(int userId, UserState newState)
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
        

        [SReportsAutorize]
        public ActionResult UserProfile(int userId)
        {
            UserDataOut user = userBLL.GetById(userId);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.Roles = SingletonDataContainer.Instance.GetRoles();
            return View("User", userBLL.GetUserForEdit(userId));
        }

        /*

        [SReportsAutorize]
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

            if (!oldPassword.Equals(user.Password))
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
        */

        [SReportsAutorize]
        public ActionResult CheckUsername(string username, string currentUsername)
        {
            if (username == currentUsername) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            bool isValid = userBLL.IsUsernameValid(username);
            return isValid ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [SReportsAutorize]
        public ActionResult CheckEmail(string email, string currentEmail)
        {
            if (email == currentEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            bool isValid = userBLL.IsEmailValid(email);
            return isValid ? new HttpStatusCodeResult(HttpStatusCode.OK) : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}