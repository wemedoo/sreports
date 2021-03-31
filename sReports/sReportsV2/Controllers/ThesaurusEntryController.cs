using AutoMapper;
using DocumentsParser.Csv.ThesaurusTranslation;
using Microsoft.VisualBasic.FileIO;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
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
using sReportsV2.Models.Common;
using sReportsV2.Models.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace sReportsV2.Controllers
{
    public class ThesaurusEntryController : BaseController
    {
        private readonly IThesaurusEntryService thesaurusEntryService;
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        private readonly IEncounterService encounterService;

        //private Dictionary<string, string> dictionary;
        public ThesaurusEntryController()
        {
            this.thesaurusEntryService = new ThesaurusEntryService();
            this.organizationService = new OrganizationService();
            this.userService = new UserService();
            this.encounterService = new EncounterService();
        }

        [Authorize]
        public ActionResult GetAll(ThesaurusEntryFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(ThesaurusEntryFilterDataIn dataIn)
        {
            ThesaurusEntryFilterData filterData = Mapper.Map<ThesaurusEntryFilterData>(dataIn);
            PaginationDataOut<ThesaurusEntryViewModel, DataIn> result = new PaginationDataOut<ThesaurusEntryViewModel, DataIn>()
            {
                Count = (int)this.thesaurusEntryService.GetAllEntriesCount(filterData),
                Data = Mapper.Map<List<ThesaurusEntryViewModel>>(this.thesaurusEntryService.GetAll(filterData)),
                DataIn = dataIn
            };
            return PartialView("ThesaurusEntryTable", result);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            return View("Edit");
        }

        [Authorize]
        public ActionResult Edit(string thesaurusEntryId)
        {
            if (!thesaurusEntryService.ExistsThesaurusEntry(thesaurusEntryId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ThesaurusEntry thesaurusEntry = thesaurusEntryService.GetById(thesaurusEntryId);
            ThesaurusEntryViewModel viewModel = Mapper.Map<ThesaurusEntryViewModel>(thesaurusEntry);
            SetThesaurusVersions(thesaurusEntry, viewModel);
            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            viewModel.Codes = GetCodes(thesaurusEntry);

            return View("Edit", viewModel);
        }


        [Authorize]
        public ActionResult EditByO4MtId(string id)
        {
            if (!thesaurusEntryService.ExistsThesaurusEntryByO4MtId(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ThesaurusEntry thesaurusEntry = thesaurusEntryService.GetByO4MtIdId(id);
            ThesaurusEntryViewModel viewModel = Mapper.Map<ThesaurusEntryViewModel>(thesaurusEntry);
            SetThesaurusVersions(thesaurusEntry, viewModel);

            ViewBag.CodeSystems = SingletonDataContainer.Instance.GetCodeSystems();
            viewModel.Codes = GetCodes(thesaurusEntry);

            return View("Edit", viewModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create([System.Web.Http.FromBody]ThesaurusEntryDataIn thesaurusEntryDTO)
        {
            UserData userData = Mapper.Map<UserData>(userCookieData);
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusEntryDTO);
            string result;

            try
            {
                result = thesaurusEntryService.InsertOrUpdate(thesaurusEntry, userData);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return Content(result);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string thesaurusEntryId, DateTime lastUpdate)
        {
            try
            {
                thesaurusEntryService.Delete(thesaurusEntryId, lastUpdate);
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

        [Authorize]
        public ActionResult InsertNewThesaurusFromForm()
        {
            return View();
        }

        [Authorize]
        public ActionResult InsertThesaurusFromForm(string formId)
        {
            List<string> insertedData = new List<string>();
            FormService formService = new FormService();
            Form form = formService.GetForm(formId);
            if(form == null) 
            { 
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            List<ThesaurusEntry> forInsert = new List<ThesaurusEntry>();
            ThesaurusEntry formThesaurus = CreateThesaurus(form.ThesaurusId, form.Title, form.Language);
            if(formThesaurus != null)
            {
                forInsert.Add(formThesaurus);
            }
            foreach (FormChapter chapter in form.Chapters)
            {
                ThesaurusEntry chapterThesaurs = CreateThesaurus(chapter.ThesaurusId, chapter.Title, form.Language, chapter.Description);
                if(chapterThesaurs != null && !forInsert.Any(x => x.O40MTId.Equals(chapterThesaurs.O40MTId)))
                {
                    forInsert.Add(chapterThesaurs);
                }
                foreach (FormPage formPage in chapter.Pages)
                {
                    ThesaurusEntry pageEntry = CreateThesaurus(formPage.ThesaurusId, formPage.Title, form.Language, formPage.Description);
                    if (pageEntry != null && !forInsert.Any(x => x.O40MTId.Equals(pageEntry.O40MTId)))
                    {
                        forInsert.Add(pageEntry);
                    }
                    foreach (List<FieldSet> listOfFS in formPage.ListOfFieldSets)
                    {
                        foreach(FieldSet fs in listOfFS)
                        {
                            ThesaurusEntry fsEntry = CreateThesaurus(fs.ThesaurusId, fs.Label, form.Language, fs.Description);
                            if (fsEntry != null && !forInsert.Any(x => x.O40MTId.Equals(fsEntry.O40MTId)))
                            {
                                forInsert.Add(fsEntry);
                            }
                            foreach (Field field in fs.Fields)
                            {
                                ThesaurusEntry fieldEntry = CreateThesaurus(field.ThesaurusId, field.Label, form.Language, field.Description);
                                if (fieldEntry != null && !forInsert.Any(x => x.O40MTId.Equals(fieldEntry.O40MTId)))
                                {
                                    forInsert.Add(fieldEntry);
                                }

                                if (field is FieldSelectable)
                                {
                                    foreach (FormFieldValue value in ((FieldSelectable)field).Values)
                                    {
                                        ThesaurusEntry valueEntry = CreateThesaurus(value.ThesaurusId, value.Label, form.Language);
                                        if (valueEntry != null && !forInsert.Any(x => x.O40MTId.Equals(valueEntry.O40MTId)))
                                        {
                                            forInsert.Add(valueEntry);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                if(forInsert.Any(x => string.IsNullOrWhiteSpace(x.O40MTId))) { throw new ArgumentNullException(); }
            }
            var result = forInsert.OrderBy(x => Int32.Parse(x.O40MTId)).ToList();
            foreach(ThesaurusEntry te in forInsert.Where(x => x != null))
            {
                if(!insertedData.Any(x => x.Equals(te.O40MTId)))
                {
                    thesaurusEntryService.InsertThesaurusEntryWithId(te, Mapper.Map<UserData>(userCookieData));
                    insertedData.Add(te.O40MTId);
                }
            }

            List<ThesaurusEntry> instertedThesaurus = forInsert.Where(x => insertedData.Contains(x.O40MTId)).ToList();

            return View("InsertedThesaurusFromForm", instertedThesaurus);
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
        public ActionResult GetEntriesCount()
        {
            ThesaurusEntryCountDataOut result = new ThesaurusEntryCountDataOut()
            {
                Total = this.thesaurusEntryService.GetAllEntriesCount(null),
                TotalUmls = this.thesaurusEntryService.GetUmlsEntriesCount()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<O4CodeableConceptDataOut> GetCodes(ThesaurusEntry thesaurusEntry) 
        {
            List<O4CodeableConceptDataOut> result = new List<O4CodeableConceptDataOut>();
            if (thesaurusEntry.Codes != null)
            {
                foreach (O4CodeableConcept code in thesaurusEntry.Codes)
                {
                    O4CodeableConceptDataOut codeDataOut = new O4CodeableConceptDataOut()
                    {
                        System = new CodeSystemDataOut()
                        {
                            Value = code.System,
                            Label = SingletonDataContainer.Instance.GetCodeSystems().FirstOrDefault(x => x.Value.Equals(code.System))?.Label,
                            Id = SingletonDataContainer.Instance.GetCodeSystems().FirstOrDefault(x => x.Value.Equals(code.System))?.Id
                        },
                        Version = code.Version,
                        Code = code.Code,
                        Value = code.Value,
                        VersionPublishDate = code.VersionPublishDate
                    };

                    result.Add(codeDataOut);
                }
            }

            return result;
        }

        private void SetThesaurusVersions(ThesaurusEntry thesaurusEntry, ThesaurusEntryViewModel viewModel)
        {
            if (thesaurusEntry.AdministrativeData != null && thesaurusEntry.AdministrativeData.VersionHistory != null)
            {
                viewModel.AdministrativeData = new AdministrativeDataViewModel();
                foreach (Domain.Entities.Common.Version version in thesaurusEntry.AdministrativeData.VersionHistory)
                {
                    viewModel.AdministrativeData.VersionHistory.Add(new Models.Common.VersionViewModel()
                    {
                        CreatedOn = version.CreatedOn,
                        RevokedOn = version.RevokedOn,
                        Id = version.Id,
                        User = Mapper.Map<UserDataViewModel>(userService.GetById(version.UserRef)),
                        Organization = Mapper.Map<OrganizationDataOut>(organizationService.GetOrganizationById(version.OrganizationRef))
                    });
                }
            }
        }        
    }
}