using AutoMapper;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MongoDB.Driver.Core.Misc;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DataIn;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class ThesaurusGlobalController : BaseController
    {
        private readonly IGlobalUserBLL globalUserBLL;
        private readonly IThesaurusEntryBLL thesaurusBLL;
        private readonly IOrganizationBLL organizationBLL;

        public ThesaurusGlobalController(IGlobalUserBLL globalUserBLL, IThesaurusEntryBLL thesaurusBLL, IOrganizationBLL organizationBLL)
        {
            this.globalUserBLL = globalUserBLL;
            this.thesaurusBLL = thesaurusBLL;
            this.organizationBLL = organizationBLL;
        }

        public ActionResult Index()
        {
            return View("~/Views/ThesaurusGlobal/Home/Home.cshtml");
        }

        [HttpPost]
        public ActionResult RegisterUser(GlobalThesaurusUserDataIn userDataIn)
        {
            userDataIn.Source = GlobalUserSource.Internal;
            if (globalUserBLL.ExistByEmailAndSource(userDataIn.Email, userDataIn.Source.Value))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "User with given email is already registered!");
            }
            globalUserBLL.InsertOrUpdate(userDataIn);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult Login(UserLoginDataIn userDataIn)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "ThesaurusGlobal");
            }
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                GlobalThesaurusUserDataOut user = globalUserBLL.TryLoginUser(userDataIn.Username, userDataIn.Password);
                if (user != null)
                {
                    SignInUser(user);
                    if (!string.IsNullOrEmpty(userDataIn.ReturnUrl))
                    {
                        return Redirect(userDataIn.ReturnUrl);
                    } 
                    else
                    {
                        return RedirectToAction("Index", "ThesaurusGlobal");
                    }
                }
            }

            ViewBag.IsLogin = true;
            ModelState.AddModelError("General", "Invalid Username or Password");

            return View("~/Views/User/LoginGlobal.cshtml");
        }

        public ActionResult Browse(DTOs.DTOs.GlobalThesaurus.DataIn.GlobalThesaurusFilterDataIn filter)
        {
            ViewBag.FilterData = filter;
            return View("~/Views/ThesaurusGlobal/Search/Browse.cshtml");
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        public ActionResult Create(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();

            return View("~/Views/ThesaurusGlobal/Create/Create.cshtml", dataOut);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public ActionResult Create(ThesaurusEntryDataIn thesaurusDataIn)
        {
            int result = thesaurusBLL.TryInsertOrUpdate(thesaurusDataIn, Mapper.Map<UserData>(userCookieData));
            
            return Content(result.ToString());
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public ActionResult SubmitConnectionWithOntology(ThesaurusEntryDataIn thesaurusDataIn)
        {
            int result = thesaurusBLL.UpdateConnectionWithOntology(thesaurusDataIn, Mapper.Map<UserData>(userCookieData));

            return Content(result.ToString());
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public ActionResult CreateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            ResourceCreatedDTO result = this.thesaurusBLL.TryInsertOrUpdateCode(codeDataIn, thesaurusEntryId);

            return new JsonResult()
            {
                Data = result
            };
        }


        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpDelete]
        public ActionResult DeleteCode(int thesaurusId, int codeId)
        {
            thesaurusBLL.DeleteCode(codeId);

            return Content(thesaurusId.ToString());
        }

        public ActionResult PreviewThesaurus(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = this.thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();

            return View("~/Views/ThesaurusGlobal/Preview/ThesaurusPreview.cshtml", dataOut);
        }

        public ActionResult ReloadThesaurus(DTOs.DTOs.GlobalThesaurus.DataIn.GlobalThesaurusFilterDataIn filterDataIn) 
        {
            var result = thesaurusBLL.ReloadThesaurus(filterDataIn);
            ViewBag.FilterData = filterDataIn;

            return PartialView("~/Views/ThesaurusGlobal/Search/GlobalThesaurusTable.cshtml", result);
        }


        [HttpPost]
        public ActionResult ReloadCodes(CodesFilterDataIn filterDataIn)
        {
            PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> result = null;

            if (filterDataIn.Id != null)
            {
                result = thesaurusBLL.ReloadCodes(filterDataIn);
            }

            return PartialView("~/Views/ThesaurusGlobal/Preview/CodesTable.cshtml", result);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        public ActionResult ContributeToTranslation(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut thesaurusDataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.ReturnUrl = filterDataIn.ReturnUrl;

            return View("~/Views/ThesaurusGlobal/Create/ContributeToTranslation.cshtml", thesaurusDataOut);
        }

        [Authorize(Roles = SmartOncologyRoleNames.EditorOrCurator)]
        [HttpPost]
        public ActionResult ContributeToTranslation(ThesaurusEntryTranslationDataIn thesaurusEntryTranslationDataIn)
        {
            sReportsV2.Common.Extensions.Ensure.IsNotNull(thesaurusEntryTranslationDataIn, nameof(thesaurusEntryTranslationDataIn));
            ThesaurusEntryDataOut thesaurusDataOut = thesaurusBLL.UpdateTranslation(thesaurusEntryTranslationDataIn, Mapper.Map<UserData>(userCookieData));

            ViewBag.CurrentTargetLanguage = thesaurusEntryTranslationDataIn.Language;
            return View("~/Views/ThesaurusGlobal/Create/ContributeToTranslation.cshtml", thesaurusDataOut);
        }

        public ActionResult Technology()
        {
            return View("~/Views/ThesaurusGlobal/GeneralInfo/Technology.cshtml");
        }

        public ActionResult Contact()
        {
            ViewBag.ReCaptchaSiteKey = ConfigurationManager.AppSettings["ReCaptchaSiteKey"];
            return View("~/Views/ThesaurusGlobal/GeneralInfo/Contact.cshtml");
        }

        [HttpPost]
        public ActionResult Contact(ContactFormDataIn contactFormData)
        {
            string secretKey = ConfigurationManager.AppSettings["ReCaptchaSecretKey"];
            bool reCaptchaInputValid = globalUserBLL.IsReCaptchaInputValid(Request.Form["g-recaptcha-response"], secretKey);

            if (reCaptchaInputValid)
            {
                globalUserBLL.SubmitContactForm(contactFormData);
            }
            return Json(new { reCaptchaInputValid });
        }

        public ActionResult TermsAndConditions()
        {
            return View("~/Views/ThesaurusGlobal/GeneralInfo/TermsAndConditions.cshtml");
        }

        public ActionResult GetTotalChartData()
        {
            ThesaurusGlobalCountDataOut result = thesaurusBLL.GetThesaurusGlobalChartData();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = SmartOncologyRoleNames.SuperAdministrator)]
        public ActionResult Users()
        {
            var data = globalUserBLL.GetUsers();
            ViewBag.Roles = globalUserBLL.GetRoles().Skip(1);
            return View("~/Views/ThesaurusGlobal/Administration/UserList.cshtml", data);
        }

        public ActionResult ActivateUser(string email)
        {
            globalUserBLL.ActivateUser(email);

            return RedirectToAction("Login", "User");
        }

        [HttpPut]
        public ActionResult SetUserStatus(int userId, GlobalUserStatus newStatus)
        {
            globalUserBLL.SetUserStatus(userId, newStatus);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPut]
        public ActionResult UpdateUserRoles(GlobalThesaurusUserDataIn userDataIn)
        {
            globalUserBLL.UpdateRoles(userDataIn);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        private void SignInUser(GlobalThesaurusUserDataOut user) 
        {
            globalUserBLL.ActivateUser(user.Email);
            var claimsIdentity = new ClaimsIdentity(user.GetClaims(),
            CookieAuthenticationDefaults.AuthenticationType);

            //This uses OWIN authentication
            HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false },
                   claimsIdentity);
        }
    }
}