using AutoMapper;
using iText.Kernel.Pdf;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class PdfController //: FormCommonController
    {
/*        private readonly string basePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        private readonly IThesaurusEntryService thesaurusService;
        private IEnumService identifierType;

        public PdfController()
        {
            organizationService = new OrganizationService();
            userService = new UserService();
            thesaurusService = new ThesaurusEntryService();
            identifierType = new EnumService();
        }

        public ActionResult GetPdf(string formId)
        {
            Form form = formService.GetForm(formId);
            PdfGenerator pdfGenerator = new PdfGenerator(form, basePath)
            {
                Organization = organizationService.GetOrganizationById(userCookieData.ActiveOrganization),
                User = userService.GetByUsername(userCookieData.Username)
            };
            
            return new FileContentResult(pdfGenerator.Generate(), "application/pdf");
        }

        public ActionResult GetPdfForFormId(string formId)
        {
            Form form = formService.GetForm(formId);
            if(form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, formId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
             
            ThesaurusEntry thesaurusEntry = thesaurusService.GetByO4MtIdId(form.ThesaurusId);
            PdfGenerator pdfGenerator = new PdfGenerator(form, basePath)
            {
                Organization = organizationService.GetOrganizationById(userCookieData?.ActiveOrganization),
                User = new User()
                {
                    FirstName = userCookieData?.FirstName ?? string.Empty,
                    LastName = userCookieData?.LastName ?? string.Empty,
                    ActiveLanguage = userCookieData?.ActiveLanguage ?? string.Empty
                },
                Definition = thesaurusEntry != null ? thesaurusEntry.Translations.FirstOrDefault(x => x.Language.Equals(form.Language))?.PreferredTerm : "",
                Translations = new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "Language", sReportsV2.Resources.TextLanguage.ResourceManager.GetString(form.Language) },
                    { "PostingDateTranslation", sReportsV2.Resources.TextLanguage.PostingDate },
                    { "VersionTranslation", sReportsV2.Resources.TextLanguage.Version },
                    { "LanguageTranslation", sReportsV2.Resources.TextLanguage.Language},
                    { "NotesTranslation", sReportsV2.Resources.TextLanguage.Notes},
                    { "DateTranslation", sReportsV2.Resources.TextLanguage.Date}
                }


            };
            return File(pdfGenerator.Generate(), "application/pdf", form.Title + ".pdf");
        }

        [SReportsAutorize]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [SReportsAutorize]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserData userData = Mapper.Map<UserData>(userCookieData);


            using (PdfReader reader = new PdfReader(file.InputStream))
            {

                using (PdfDocument pdfDocument = new PdfDocument(reader))
                {
                    var formId = pdfDocument.GetDocumentInfo().GetMoreInfo("formId");
                    Form form = formService.GetForm(formId);
                    if (form == null)
                    {
                        Log.Warning(SReportsResource.FormNotExists, 404, formId);
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }

                    PdfFormParser parser = new PdfFormParser(form, pdfDocument, identifierType.GetAll().Where(x => x.Type == IdentifierKind.PatientIdentifierType.ToString()).ToList());
                    Form parsedForm = parser.ReadFieldsFromPdf();
                    FormInstance parsedFormInstance = new FormInstance(parsedForm);
                    parsedFormInstance.Fields = parser.Fields;
                    parsedFormInstance.Date = parsedForm.Date;
                    parsedFormInstance.Notes = parsedForm.Notes;
                    parsedFormInstance.FormState = parsedForm.FormState;
                    parsedFormInstance.Referrals = new System.Collections.Generic.List<string>();
                    SetPatientRelatedData(form, parsedFormInstance, parser.Patient, userData);


                    InsertFormInstance(parsedFormInstance);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        private void SetPatientRelatedData(Form form, FormInstance parsedFormInstance, PatientEntity patient, UserData user)
        {
            if (!form.DisablePatientData)
            {
                string patientId = InsertPatient(patient);

                string episodeOfCareId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Pdf", parsedFormInstance.Date.Value, user);
                string encounterId = InsertEncounter(episodeOfCareId);
                parsedFormInstance.EncounterRef = encounterId;
                parsedFormInstance.EpisodeOfCareRef = episodeOfCareId;
                parsedFormInstance.PatientRef = patientId;
            }
        }

        private string InsertFormInstance(FormInstance formInstance)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            formInstance.UserRef = userCookieData.Id;
            formInstance.Language = userCookieData.ActiveLanguage;
            formInstance.OrganizationRef = userCookieData.ActiveOrganization;
            return formInstanceService.InsertOrUpdate(formInstance);
        }*/
    }
}