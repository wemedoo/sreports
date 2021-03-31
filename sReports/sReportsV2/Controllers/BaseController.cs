using AutoMapper;
using Serilog;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Organization;
using sReportsV2.Models.User;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace sReportsV2.Controllers
{
    public class BaseController : Controller
    {
        protected UserCookieData userCookieData;
        private IUserService userService;
        private IOrganizationService organizationService;
        public BaseController()
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
            {
                userCookieData = System.Web.HttpContext.Current.Session["userData"] as UserCookieData;
                if (userCookieData == null)
                {
                    userService = new UserService();
                    organizationService = new OrganizationService();

                    User userEntity = userService.GetByUsername(System.Web.HttpContext.Current.User.Identity.Name);
                    userCookieData = Mapper.Map<UserCookieData>(userEntity);
                    userCookieData.Organizations = Mapper.Map<List<OrganizationDataOut>>(this.organizationService.GetOrganizationsByIds(userEntity.OrganizationRefs));
                    System.Web.HttpContext.Current.Session["userData"] = userCookieData;
                }
                ViewBag.UserCookieData = userCookieData;
            }
            ViewBag.Languages = SingletonDataContainer.Instance.GetLanguages();

        }

        public ActionResult NotFound(string resourceId)
        {
            Log.Warning(SReportsResource.FormNotExists, 404, resourceId);
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}