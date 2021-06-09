using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.DFD;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using sReportsV2.SqlDomain.Interfaces;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {
        private readonly IEncounterDAL encounterDAL;
        private readonly IThesaurusMergeService thesaurusMergeService;
        private readonly IFormDAL formService;
        private readonly IFormInstanceDAL formInstanceService;
        private readonly IFormDistributionService formDistributionService;
        private readonly IDFDService dfdService;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IThesaurusEntryBLL thesaurusEntryBLL;

        //private Dictionary<string, string> dictionary;
        public ThesaurusEntryController(IEpisodeOfCareDAL episodeOfCareDAL, IThesaurusEntryBLL thesaurusEntryBLL, IEncounterDAL encounterDAL)
        {
            this.encounterDAL = encounterDAL;
            this.thesaurusMergeService = new ThesaurusMergeService();
            this.formService = new FormDAL();
            this.formInstanceService = new FormInstanceDAL();
            this.formDistributionService = new FormDistributionService();
            this.dfdService = new DFDService();
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.thesaurusEntryBLL = thesaurusEntryBLL;
           
        }


        [SReportsAutorize]
        public ActionResult GetAll(ThesaurusEntryFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        //done
        [SReportsAutorize]
        public ActionResult ReloadTable(ThesaurusEntryFilterDataIn dataIn)
        {
            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = thesaurusEntryBLL.ReloadTable(dataIn);
            return PartialView("ThesaurusEntryTable", result);
        }

        [SReportsAutorize]
        public ActionResult ThesaurusProperties(int? o4mtid)
        {
            ThesaurusEntryDataOut viewModel = this.thesaurusEntryBLL.GetThesaurusDataOut(o4mtid.Value);
            return PartialView(viewModel);
        }

        //done
        [SReportsAutorize]
        public ActionResult ReloadSearchTable(ThesaurusEntryFilterDataIn dataIn)
        {
            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = thesaurusEntryBLL.ReloadTable(dataIn);
            ViewBag.ActiveThesaurus = dataIn.ActiveThesaurus;
            return PartialView(result);
        }

        [Authorize]
        [SReportsAutorize]
        public ActionResult GetReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus = thesaurusEntryBLL.GetById(filter.Id);

            ViewBag.O4MTId = thesaurus.Id;
            ViewBag.Id = filter.Id;
            ViewBag.CurrentThesaurus = thesaurus;
            ViewBag.FilterData = filter;
            ViewBag.PreferredTerm = thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);

            return View("ReviewTree", GetReviewTreeDataOut(filter, thesaurus));
        }

        [SReportsAutorize]
        public ActionResult GetThesaurusInfo(int id)
        {
            return PartialView("ThesaurusInfo", Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntryBLL.GetById(id)));
        }

        [SReportsAutorize]
        public ActionResult ReloadReviewTree(ThesaurusReviewFilterDataIn filter)
        {
            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus = thesaurusEntryBLL.GetById(filter.Id);

            ViewBag.O4MTId = thesaurus.Id;
            ViewBag.PreferredTerm = string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage) : filter.PreferredTerm;
            ViewBag.Id = filter.Id;
            ViewBag.FilterData = filter;

            return PartialView("ThesaurusReviewList", GetReviewTreeDataOut(filter, thesaurus));
        }

        private PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus) 
        {
            PaginationDataOut<ThesaurusEntryDataOut, DataIn>  result = thesaurusEntryBLL.GetReviewTreeDataOut(filter, thesaurus, userCookieData);
            ViewBag.Thesaurus = Mapper.Map<ThesaurusEntryDataOut>(thesaurus);

            return result;
        }

        [SReportsAutorize]
        public ActionResult Create()
        {
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            return View("Edit");
        }

        //done
       [SReportsAutorize]
        public ActionResult Edit(int thesaurusEntryId)
        {
            return GetThesaurusEditById(thesaurusEntryId);
        }

        //done
        [SReportsAutorize]
        public ActionResult EditByO4MtId(int id)
        {
            return GetThesaurusEditById(id);
        }

        //done
        [HttpPost]
        [SReportsAutorize]
        [SReportsThesaurusValidate]
        public ActionResult Create([System.Web.Http.FromBody]ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            string result = thesaurusEntryBLL.CreateThesaurus(thesaurusEntryDTO, userCookieData);

            return Content(result);
        }

        //done
        [HttpPost]
        [SReportsAutorize]
        public ActionResult CreateByPreferredTerm(string preferredTerm, string description)
        {
            string o4mtId = string.Empty;
            ThesaurusEntry thesaurusEntry = new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
            };
            thesaurusEntry.SetPrefferedTermAndDescriptionForLang(userCookieData.ActiveLanguage, preferredTerm, description);
                    
            try
            {
                o4mtId = thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(thesaurusEntry), userCookieData);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return Content(o4mtId);
        }


        //done
        [SReportsAutorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(int thesaurusEntryId)
        {
            try
            {
                thesaurusEntryBLL.TryDelete(thesaurusEntryId);
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

        /* public ActionResult LoadTranslations()
         {
             UserData userData = Mapper.Map<UserData>(userCookieData);
             ParserThesaurusTranslation.ParseAndUpdateThesaurus(userData);

             return new HttpStatusCodeResult(HttpStatusCode.NoContent);
         }*/
        [SReportsAutorize]
        public ActionResult ThesaurusMoreContent(int id)
        {
            ThesaurusEntryDataOut viewModel = Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntryBLL.GetById(id));

            return PartialView("ThesaurusMoreContent", viewModel);
        }


        [SReportsAutorize]
        public ActionResult InsertNewThesaurusFromForm()
        {
            return View();
        }

        //done
        public ActionResult GetEntriesCount()
        {
            ThesaurusEntryCountDataOut result = thesaurusEntryBLL.GetEntriesCount();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SReportsAutorize]
        public ActionResult ThesaurusPreview(int? o4mtid, string activeThesaurus)
        {
            ThesaurusEntryDataOut viewModel = thesaurusEntryBLL.GetThesaurusDataOut(o4mtid.Value);
            ViewBag.ActiveThesaurus = activeThesaurus;
            return PartialView(viewModel);
        }

        private ActionResult GetThesaurusEditById(int thesaurusEntryId)
        {
            if (!thesaurusEntryBLL.ExistsThesaurusEntry(thesaurusEntryId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurusEntry = thesaurusEntryBLL.GetById(thesaurusEntryId);
            ThesaurusEntryDataOut viewModel = Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntry);
            thesaurusEntryBLL.SetThesaurusVersions(thesaurusEntry, viewModel);


            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            ViewBag.TotalAppeareance = formService.GetThesaurusAppereanceCount(thesaurusEntry.Id, string.Empty);
            return View("Edit", viewModel);
        }
        private ThesaurusEntry CreateThesaurus(string thesaurusId, string label, string language, string description = null)
        {
            ThesaurusEntry result = null;
            if (!string.IsNullOrWhiteSpace(thesaurusId))
            {

                var translations = new List<ThesaurusEntryTranslation>();
                translations.Add(new ThesaurusEntryTranslation()
                {
                    PreferredTerm = label != null ? label : string.Empty,
                    Definition = string.IsNullOrWhiteSpace(description) ? "." : description,
                    Language = language
                });
                result = new ThesaurusEntry
                {
                    O40MTId = thesaurusId,
                    Translations = translations
                };
            }
            return result;

        }
    }
}