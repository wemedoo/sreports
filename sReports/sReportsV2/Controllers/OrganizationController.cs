using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Serilog;
using System.Web.Mvc;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;

namespace sReportsV2.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController()
        {
            organizationService = new OrganizationService();
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetOrganizationIdentifierTypes();
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetOrganizationTypes();

            return View("Organization");
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(OrganizationDataIn organization)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                organizationService.Insert(Mapper.Map<Organization>(organization));
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
        public ActionResult GetAll(DataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(DataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            PaginationDataOut<OrganizationDataOut, DataIn> result = new PaginationDataOut<OrganizationDataOut, DataIn>()
            {
                Count = (int)this.organizationService.GetAllEntriesCount(),
                Data = Mapper.Map<List<OrganizationDataOut>>(this.organizationService.GetAll(dataIn.PageSize, dataIn.Page)),
                DataIn = dataIn
            };
            return PartialView("OrganizationEntryTable", result);
        }

        public ActionResult GetUsersByOrganizationCount()
        {
            return PartialView(Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationService.GetOrganizationUsersCount()));
        }

        [Authorize]
        public ActionResult Edit(string organizationId)
        {
            Organization organization = organizationService.GetOrganizationById(organizationId);
            if (organization == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            OrganizationDataOut result = Mapper.Map<OrganizationDataOut>(organization);
            result.Identifiers = GetOrganizationIdentifiersDataOut(organization.Identifiers);
            result.PartOf = organization.PartOf != null ? Mapper.Map<OrganizationDataOut>(organizationService.GetOrganizationById(organization.PartOf)) : null;
            
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetOrganizationIdentifierTypes();
            ViewBag.OrganizationTypes = SingletonDataContainer.Instance.GetOrganizationTypes();
            return View("Organization", result);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string organizationId, DateTime lastUpdate)
        {
            try
            {
                this.organizationService.Delete(organizationId, lastUpdate);
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

        public ActionResult ExistIdentifier(IdentifierDataIn dataIn)
        {
            return Json(!organizationService.ExistsOrganizationByIdentifier(Mapper.Map<IdentifierEntity>(dataIn)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadHierarchy(string partOf)
        {
            List<OrganizationDataOut> hierarchy = new List<OrganizationDataOut>();
            Organization parent = this.organizationService.GetOrganizationById(partOf);
            if(parent != null && parent.Ancestors != null)
            {
                hierarchy = Mapper.Map<List<OrganizationDataOut>>(this.organizationService.GetOrganizationsByIds(parent.Ancestors));
            }
            hierarchy.Add(Mapper.Map<OrganizationDataOut>(parent));

            return PartialView("OrganizationStructure", hierarchy);
        }

        public ActionResult GetAutocompleteData(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<AutocompleteDataOut> organizationDataOuts = new List<AutocompleteDataOut>();

            organizationDataOuts = organizationService.SearchByName(dataIn.Term, dataIn.Page, 10)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(organizationService.GetSearchByNameCount(dataIn.Term) / 10.00) > dataIn.Page,
                },
                results = organizationDataOuts

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<IdentifierDataOut> GetOrganizationIdentifiersDataOut(List<IdentifierEntity> identifiers)
        {
            List<IdentifierDataOut> result = new List<IdentifierDataOut>();

            if (identifiers != null)
            {
                foreach (IdentifierEntity identifier in identifiers)
                {
                    string systemName = SingletonDataContainer.Instance.GetOrganizationIdentifierTypes().FirstOrDefault(x => x.O4MtId.Equals(identifier.System))?.Name;
                    IdentifierDataOut organizationIdentifier = new IdentifierDataOut()
                    {
                        System = new IdentifierTypeDataOut(identifier.System, systemName),
                        Type = identifier.Type,
                        Use = identifier.Use,
                        Value = identifier.Value
                    };

                    result.Add(organizationIdentifier);
                }
            }
            return result;
        }
    }
}