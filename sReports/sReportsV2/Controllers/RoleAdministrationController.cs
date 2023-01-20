using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class RoleAdministrationController : BaseController
    {
        private readonly IRoleBLL roleBLL;
        public RoleAdministrationController(IRoleBLL roleBLL)
        {
            this.roleBLL = roleBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll()
        {
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(DataIn dataIn)
        {
            var result = roleBLL.GetAll(dataIn);
            return PartialView("RoleEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        public ActionResult Create()
        {
            return View("Edit");
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult Edit(int roleId)
        {
            var result = roleBLL.GetById(roleId);
            ViewBag.Modules = roleBLL.GetAllModules();
            return View(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [HttpPost]
        public ActionResult Edit(RoleDataIn roleDataIn)
        {
            var response = roleBLL.InsertOrUpdate(roleDataIn);
            UpdateUserCookie(userCookieData.Email);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}