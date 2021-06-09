using AutoMapper;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MongoDB.Driver.Core.Misc;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class ThesaurusGlobalController : BaseController
    {
        IGlobalUserDAL globalUserDAL;

        protected IUserBLL userBLL;
        protected IThesaurusEntryBLL thesaurusBLL;
        protected IOrganizationBLL organizationBLL;

        public ThesaurusGlobalController()
        {
           
        }

        public ThesaurusGlobalController(IUserBLL userBLL, IOrganizationBLL organizationBLL, IThesaurusEntryBLL thesaurusBLL)
        {
            globalUserDAL = DependencyResolver.Current.GetService<IGlobalUserDAL>();
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
            this.thesaurusBLL = thesaurusBLL;
        }
        // GET: ThesaurusGlobal
        public ActionResult Index()
        {
            return View("~/Views/ThesaurusGlobal/Home/Home.cshtml");
        }

        [HttpPost]
        public ActionResult RegisterUser(GlobalThesaurusUserDataIn userDataIn)
        {
            userDataIn.Source = GlobalUserSource.Internal;
            if (globalUserDAL.ExistByEmailAndSource(userDataIn.Email, userDataIn.Source.Value))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "User alredy registred!");
            }
            globalUserDAL.InsertOrUpdate(Mapper.Map<GlobalThesaurusUser>(userDataIn));
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult Login(UserLoginDataIn userDataIn)
        {
            if (User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));

            if (ModelState.IsValid)
            {
                if (this.globalUserDAL.IsValidUser(userDataIn.Username, userDataIn.Password))
                {
                    SignInUser(userDataIn);
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
                }
            }

            ModelState.AddModelError("General", "Invalid Username or Password");

            return View("~/Views/User/LoginGlobal.cshtml");
        }

        public ActionResult Browse(DTOs.DTOs.GlobalThesaurus.DataIn.GlobalThesaurusFilterDataIn filter)
        {
            ViewBag.FilterData = filter;
            return View("~/Views/ThesaurusGlobal/Search/Browse.cshtml");
        }

        [Authorize]
        public ActionResult Create(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = thesaurusBLL.GetThesaurusByFilter(filterDataIn);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();

            return View("~/Views/ThesaurusGlobal/Create/Create.cshtml", dataOut);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(ThesaurusEntryDataIn thesaurusDataIn)
        {
            int result = thesaurusBLL.TryInsertOrUpdate(thesaurusDataIn);
            
            return Content(result.ToString());
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateCode(O4CodeableConceptDataIn codeDataIn, int? tid)
        {
            int result = this.thesaurusBLL.TryInsertOrUpdateCode(codeDataIn, tid);
            
            return Content(result.ToString());
        }


        [Authorize]
        [HttpDelete]
        public ActionResult DeleteCode(int thesaurusId, int codeId)
        {
            thesaurusBLL.DeleteCode(codeId);

            return Content(thesaurusId.ToString());
        }

        public ActionResult PreviewThesaurus(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = this.thesaurusBLL.GetThesaurusDataOut(filterDataIn.Id.Value);
            ViewBag.FilterData = filterDataIn;
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();

            return View("~/Views/ThesaurusGlobal/Preview/ThesaurusPreview.cshtml", dataOut);
        }

        public ActionResult ReloadThesaurus(DTOs.DTOs.GlobalThesaurus.DataIn.GlobalThesaurusFilterDataIn filterDataIn) 
        {
            var result = thesaurusBLL.ReloadThesaurus(filterDataIn);
           
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

        private List<O4CodeableConceptDataOut> GetCodes(ThesaurusEntry thesaurusEntry)
        {
            List<O4CodeableConceptDataOut> result = new List<O4CodeableConceptDataOut>();
            if (thesaurusEntry != null && thesaurusEntry.Codes != null)
            {
                foreach (O4CodeableConcept code in thesaurusEntry.Codes)
                {
                    O4CodeableConceptDataOut codeDataOut = new O4CodeableConceptDataOut()
                    {
                        System = Mapper.Map<CodeSystemDataOut>(code.System),
                        Version = code.Version,
                        Code = code.Code,
                        Value = code.Value,
                        VersionPublishDate = code.VersionPublishDate,
                        Link = code.Link,
                        EntryDateTime = code.EntryDateTime,
                        Id = code.Id
                    };

                    result.Add(codeDataOut);
                }
            }

            return result;
        }

        private void SignInUser(UserLoginDataIn userDataIn) 
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, userDataIn.Username));
            var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationType);

            //This uses OWIN authentication
            HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false },
                   claimsIdentity);
        }
    }
}