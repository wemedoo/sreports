using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Organization;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Constants;

namespace sReportsV2.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationBLL organizationBLL;

        public OrganizationController(IOrganizationBLL organizationBLL)
        {
            this.organizationBLL = organizationBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        public ActionResult Create()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationIdentifierType);
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationType);
            return View("Organization");
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsOrganizationValidate]
        public ActionResult Create(OrganizationDataIn organization)
        {
            organizationBLL.Insert(organization);
            return new HttpStatusCodeResult(HttpStatusCode.Created);       
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll(OrganizationFilterDataIn dataIn)
        {
            SetCountryNameIfFilterByCountryIsIncluded(dataIn);
            ViewBag.FilterData = dataIn;
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationType);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationIdentifierType);
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(OrganizationFilterDataIn dataIn)
        {
            var result = organizationBLL.ReloadTable(dataIn);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationIdentifierType);
            return PartialView("OrganizationEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult Edit(int organizationId)
        {
            var result = organizationBLL.GetOrganizationForEdit(organizationId);
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationType);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.OrganizationIdentifierType);
            return View("Organization", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(OrganizationDataIn organizationDataIn)
        {
            organizationBLL.Delete(organizationDataIn);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ExistIdentifier(IdentifierDataIn dataIn)
        {
            return Json(!organizationBLL.ExistIdentifier(dataIn), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadHierarchy(int? parentId)
        {
            if (parentId == null)
            {
                return PartialView("OrganizationHierarchy");
            }
            
            return PartialView("OrganizationHierarchy", organizationBLL.ReloadHierarchy(parentId));
        }

        public ActionResult GetAutocompleteData(AutocompleteDataIn dataIn)
        {
            var result = organizationBLL.GetDataForAutocomplete(dataIn);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUsersByOrganizationCount()
        {
            return PartialView(Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(null, null)));
        }

        public ActionResult GetById(int? organizationId)
        {
            if (organizationId.HasValue)
            {
                OrganizationDataOut organization = organizationBLL.GetOrganizationById(organizationId.Value);
                return Json(new { id = organization.Id, text = organization.Name }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

        }

        private void SetCountryNameIfFilterByCountryIsIncluded(OrganizationFilterDataIn dataIn)
        {
            if (dataIn != null)
            {
                string countryName = string.Empty;
                if (dataIn.CountryId.HasValue)
                {
                    countryName = SingletonDataContainer.Instance.GetCustomEnumPreferredTerm(dataIn.CountryId.Value);
                }
                dataIn.CountryName = countryName;
            }
        }
    }
}