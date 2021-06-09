using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Serilog;
using System.Web.Mvc;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.Domain.Exceptions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationBLL organizationBLL;

        public OrganizationController(IOrganizationBLL organizationBLL)
        {
            this.organizationBLL = organizationBLL;
        }

        [SReportsAutorize]
        public ActionResult Create()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.OrganizationIdentifierType).ToList();
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.OrganizationType).ToList();
            return View("Organization");
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsOrganizationValidate]
        public ActionResult Create(OrganizationDataIn organization)
        {
            organizationBLL.Insert(organization);
            return new HttpStatusCodeResult(HttpStatusCode.Created);       
        }

        [SReportsAutorize]
        public ActionResult GetAll(OrganizationFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.OrganizationType).ToList();
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.OrganizationIdentifierType).ToList();
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(OrganizationFilterDataIn dataIn)
        {
            var result = organizationBLL.ReloadTable(dataIn);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.OrganizationIdentifierType)).ToList();
            return PartialView("OrganizationEntryTable", result);
        }

        [SReportsAutorize]
        public ActionResult Edit(int organizationId)
        {
            var result = organizationBLL.GetOrganizationForEdit(organizationId);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.OrganizationIdentifierType)).ToList();
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.OrganizationType)).ToList();
            return View("Organization", result);
        }

        [SReportsAutorize]
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
    }
}