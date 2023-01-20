using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.CustomEnum;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.CodeSystem;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.Common.Helpers;

namespace sReportsV2.Controllers
{
    public class AdministrationController : BaseController
    {
        private readonly IThesaurusEntryBLL thesaurusEntryBLL;
        private readonly ICustomEnumBLL customEnumBLL;
        private readonly IThesaurusDAL thesaurusEntryDAL;

        public AdministrationController(ICustomEnumBLL customEnumBLL, IThesaurusEntryBLL thesaurusEntryBLL, IThesaurusDAL thesaurusEntryDAL)
        {
            this.thesaurusEntryBLL = thesaurusEntryBLL;
            this.customEnumBLL = customEnumBLL;
            this.thesaurusEntryDAL = thesaurusEntryDAL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Administration)]
        public ActionResult GetAll(AdministrationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.FilterData = dataIn;
            return View();
        }

        [Authorize]
        public ActionResult ReloadTable(AdministrationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            if (dataIn.PredefinedType == Common.Enums.CustomEnumType.CodeSystem)
            {
                var viewModel = GetCodeSystems(dataIn);
                return PartialView("CodeSystemTable", viewModel);
            }
            else
            {
                PaginationDataOut<CustomEnumDataOut, DataIn> result = GetPredefinedType(dataIn);
                ViewBag.PredefinedType = dataIn.PredefinedType;
                return PartialView("PredefinedTypeTable", result);
            }
        }

        [Authorize]
        public ActionResult ReloadThesaurusTable(AdministrationFilterDataIn dataIn)
        {
            var result = thesaurusEntryBLL.GetByAdministrationTerm(dataIn);
            if (result != null)
            {
                ViewBag.PredefinedType = dataIn.PredefinedType;
                return PartialView("ThesaurusTable", result);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        public ActionResult Create(AdministrationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.PredefinedType = dataIn.PredefinedType;
            ViewBag.FilterData = dataIn;
            return View("Create");
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsAdministrationValidate]
        public ActionResult Create(CustomEnumDataIn enumDataIn)
        {
            customEnumBLL.Insert(enumDataIn, userCookieData.ActiveOrganization);
            SingletonDataContainer.Instance.RefreshSingleton();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Autocomplete(string searchValue, int page)
        {
            var result = thesaurusEntryBLL.GetAll(userCookieData.ActiveLanguage, searchValue, page);
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Administration)]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(CustomEnumDataIn customEnumDataIn)
        {            
            customEnumBLL.Delete(customEnumDataIn);
            SingletonDataContainer.Instance.RefreshSingleton();
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Administration)]
        [HttpPost]
        public ActionResult CreateCodeSystem(CodeSystemDataIn codeSystem)
        {
            thesaurusEntryBLL.InsertOrUpdateCodeSystem(codeSystem);
            SingletonDataContainer.Instance.RefreshSingleton();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        private PaginationDataOut<CustomEnumDataOut, DataIn> GetPredefinedType(AdministrationFilterDataIn dataIn)
        {
            Tuple<List<CustomEnumDataOut>, int> predefinedTypes = GetPredefinedTypes(dataIn);
            PaginationDataOut<CustomEnumDataOut, DataIn> result = new PaginationDataOut<CustomEnumDataOut, DataIn>()
            {
                Count = predefinedTypes.Item2,
                Data = predefinedTypes.Item1,
                DataIn = dataIn
            };

            return result;
        }

        private Tuple<List<CustomEnumDataOut>, int> GetPredefinedTypes(AdministrationFilterDataIn dataIn)
        {
            IQueryable<CustomEnumDataOut> result = SingletonDataContainer.Instance.GetEnums()
                .Where(x => x.Type == dataIn.PredefinedType && x.OrganizationId == userCookieData.ActiveOrganization).AsQueryable();

            int count = dataIn.PreferredTerm == null ? result.Count() :
                                                       result.Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToUpper()
                                                       .Contains(dataIn.PreferredTerm.ToUpper())).Count();

            if (dataIn.PreferredTerm == null)
            {
                if (dataIn.ColumnName != null)
                    result = SortPreferredTermsByField(result, dataIn, userCookieData.ActiveLanguage);
                else
                    result = result.Skip((dataIn.Page - 1) * dataIn.PageSize).Take(dataIn.PageSize);
            }
            else
            {
                if (dataIn.ColumnName != null)
                {
                    result = result.Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToUpper()
                        .Contains(dataIn.PreferredTerm.ToUpper()));
                    result = SortPreferredTermsByField(result, dataIn, userCookieData.ActiveLanguage);
                }
                else
                {
                    result = result.Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToUpper()
                        .Contains(dataIn.PreferredTerm.ToUpper()))
                        .Skip((dataIn.Page - 1) * dataIn.PageSize)
                        .Take(dataIn.PageSize);
                }
            }

            return Tuple.Create(result.ToList(), count);
        }

        private PaginationDataOut<CodeSystemDataOut, DataIn> GetCodeSystems(AdministrationFilterDataIn dataIn)
        {
            IEnumerable<CodeSystemDataOut> codeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            if (dataIn.PreferredTerm != null)
            {
                codeSystems = codeSystems.Where(x => x.Label.ToLower().Contains(dataIn.PreferredTerm.ToLower()));
                if (dataIn.ColumnName != null)
                    codeSystems = SortCodeSystemsByField(codeSystems.AsQueryable(), dataIn);
            }

            return new PaginationDataOut<CodeSystemDataOut, DataIn>()
            {
                Count = codeSystems.Count(),
                Data = dataIn.ColumnName != null ? SortCodeSystemsByField(codeSystems.AsQueryable(), dataIn).ToList()
                               : codeSystems.Skip((dataIn.Page - 1) * dataIn.PageSize).Take(dataIn.PageSize).ToList(),
                DataIn = dataIn
            };
        }

        private IQueryable<CustomEnumDataOut> SortPreferredTermsByField(IQueryable<CustomEnumDataOut> result, AdministrationFilterDataIn dataIn, string activeLanguage)
        {
            switch (dataIn.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    if (dataIn.IsAscending)
                        return result.OrderBy(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage).ToUpper())
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                    else
                        return result.OrderByDescending(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage).ToUpper())
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                case AttributeNames.Definition:
                    if (dataIn.IsAscending)
                        return result.OrderBy(x => x.Thesaurus.GetDefinitionByTranslationOrDefault(activeLanguage).ToUpper())
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                    else
                        return result.OrderByDescending(x => x.Thesaurus.GetDefinitionByTranslationOrDefault(activeLanguage).ToUpper())
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                case AttributeNames.Label:
                    return result.OrderBy(x => x.Id)
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                case AttributeNames.SAB:
                    return result.OrderBy(x => x.Id)
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, dataIn.ColumnName, dataIn.IsAscending)
                               .Skip((dataIn.Page - 1) * dataIn.PageSize)
                               .Take(dataIn.PageSize);
            }
        }

        private IQueryable<CodeSystemDataOut> SortCodeSystemsByField(IQueryable<CodeSystemDataOut> codeSystems, AdministrationFilterDataIn dataIn) 
        {
            switch (dataIn.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    return codeSystems.OrderBy(x => x.Id)
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                case AttributeNames.Definition:
                    return codeSystems.OrderBy(x => x.Id)
                                .Skip((dataIn.Page - 1) * dataIn.PageSize)
                                .Take(dataIn.PageSize);
                default:
                    return SortTableHelper.OrderByField(codeSystems, dataIn.ColumnName, dataIn.IsAscending)
                               .Skip((dataIn.Page - 1) * dataIn.PageSize)
                               .Take(dataIn.PageSize);
            }
        }
    }
}