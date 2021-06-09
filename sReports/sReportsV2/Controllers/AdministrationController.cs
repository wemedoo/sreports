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

        [Authorize]
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
            PaginationDataOut<CustomEnumDataOut, DataIn> result = GetPredefinedType(dataIn);
            ViewBag.PredefinedType = dataIn.PredefinedType;
            return PartialView("PredefinedTypeTable", result);

        }

        [Authorize]
        public ActionResult ReloadThesaurusTable(AdministrationFilterDataIn dataIn)
        {
            var result = thesaurusEntryBLL.GetByAdministrationTerm(dataIn);
            if (result != null)
            {
                return PartialView("ThesaurusTable", result);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize]
        public ActionResult Create(AdministrationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            ViewBag.PredefinedType = dataIn.PredefinedType;
            ViewBag.FilterData = dataIn;
            return View("Create");
        }

        [Authorize]
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

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(CustomEnumDataIn customEnumDataIn)
        {            
            customEnumBLL.Delete(customEnumDataIn);
            SingletonDataContainer.Instance.RefreshSingleton();
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
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
            //TO DO FIX
            List<CustomEnumDataOut> predefinedTypes = new List<CustomEnumDataOut>();
            predefinedTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == dataIn.PredefinedType).ToList();

            return Tuple.Create(dataIn.PreferredTerm == null ? predefinedTypes
                    .Skip((dataIn.Page - 1) * dataIn.PageSize)
                    .Take(dataIn.PageSize)
                    .ToList()
                :
                    predefinedTypes.Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToUpper()
                    .Contains(dataIn.PreferredTerm.ToUpper()))
                    .Skip((dataIn.Page - 1) * dataIn.PageSize)
                    .Take(dataIn.PageSize)
                    .ToList(),

                    dataIn.PreferredTerm == null ?
                    predefinedTypes.Count
                    :
                    predefinedTypes.Where(x => x.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage).ToUpper()
                    .Contains(dataIn.PreferredTerm.ToUpper())).Count()
                    );
        }
    }
}