using AutoMapper;
using Chapters;
using iText.Kernel.Pdf;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PdfBLL : IPdfBLL
    {
        private readonly string basePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly ICustomEnumDAL customEnumDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IEncounterDAL encounterDAL;

        public PdfBLL(IThesaurusDAL thesaurusDAL, IOrganizationDAL organizationDAL, IFormDAL formDAL, ICustomEnumDAL customEnumDAL, IFormInstanceDAL formInstanceDAL, IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL, IEncounterDAL encounterDAL)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.organizationDAL = organizationDAL;
            this.formDAL = formDAL;
            this.customEnumDAL = customEnumDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.patientDAL = patientDAL;
            this.encounterDAL = encounterDAL;
        }

        public byte[] Generate(Form form, UserCookieData userCookieData, Dictionary<string, string> translatedFields)
        {
            ThesaurusEntry thesaurusEntry = thesaurusDAL.GetById(form.ThesaurusId);
            PdfGenerator pdfGenerator = new PdfGenerator(form, basePath)
            {
                Organization = organizationDAL.GetById(userCookieData.ActiveOrganization),
                User = new User()
                {
                    FirstName = userCookieData?.FirstName ?? string.Empty,
                    LastName = userCookieData?.LastName ?? string.Empty,
                },
                Definition = thesaurusEntry != null ? thesaurusEntry.Translations.FirstOrDefault(x => x.Language.Equals(form.Language))?.PreferredTerm : "",
                Translations = translatedFields
            };

            return pdfGenerator.Generate();
        }

        public void Upload(HttpPostedFileBase file, UserCookieData userCookieData)
        {
            Ensure.IsNotNull(file, nameof(file));
            UserData userData = Mapper.Map<UserData>(userCookieData);

            using (PdfReader reader = new PdfReader(file.InputStream))
            {
                using (PdfDocument pdfDocument = new PdfDocument(reader))
                {
                    var formId = pdfDocument.GetDocumentInfo().GetMoreInfo("formId");
                    Form form = formDAL.GetForm(formId);
                    Ensure.IsNotNull(form, nameof(form));

                    PdfFormParser parser = new PdfFormParser(form, pdfDocument, customEnumDAL.GetAll().Where(x => x.Type == CustomEnumType.PatientIdentifierType).ToList());
                    Form parsedForm = parser.ReadFieldsFromPdf();
                    FormInstance parsedFormInstance = new FormInstance(parsedForm);
                    parsedFormInstance.Fields = parser.Fields;
                    parsedFormInstance.Date = parsedForm.Date;
                    parsedFormInstance.Notes = parsedForm.Notes;
                    parsedFormInstance.FormState = parsedForm.FormState;
                    parsedFormInstance.Referrals = new System.Collections.Generic.List<string>();
                    SetPatientRelatedData(form, parsedFormInstance, parser.Patient, userData);

                    InsertFormInstance(parsedFormInstance, userCookieData);
                }
            }
        }

        private void SetPatientRelatedData(Form form, FormInstance parsedFormInstance, Patient patient, UserData user)
        {
            if (!form.DisablePatientData)
            {
                int patientId = InsertPatient(patient);

                int episodeOfCareId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Pdf", parsedFormInstance.Date.Value, user);
                int encounterId = InsertEncounter(episodeOfCareId);
                parsedFormInstance.EncounterRef = encounterId;
                parsedFormInstance.EpisodeOfCareRef = episodeOfCareId;
                parsedFormInstance.PatientId = patientId;
            }
        }

        private string InsertFormInstance(FormInstance formInstance, UserCookieData userCookieData)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            formInstance.UserId = userCookieData.Id;
            formInstance.Language = userCookieData.ActiveLanguage;
            formInstance.OrganizationId = userCookieData.ActiveOrganization;

            return formInstanceDAL.InsertOrUpdate(formInstance);
        }

        private int InsertPatient(Patient patient)
        {
            int patientId = 0;
            if (patient != null)
            {
                patientId = patient != null && patient.Identifiers != null && patient.Identifiers.Count > 0 ?
                    patientDAL.GetByIdentifier(patient.Identifiers[0]).Id
                    :
                    0;

                if (patientId != 0)
                {
                    patientDAL.InsertOrUpdate(patient);
                }
            }

            return patientId;
        }

        private int InsertEpisodeOfCare(int patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate, UserData user)
        {
            startDate = startDate.Date;
            EpisodeOfCare eoc;
            if (episodeOfCare != null)
            {
                eoc = Mapper.Map<EpisodeOfCare>(episodeOfCare);
                eoc.Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.DiagnosisRole = 12227;
                eoc.OrganizationId = 1;
            }
            else
            {
                eoc = new EpisodeOfCare()
                {
                    Description = $"Generated from {source}",
                    DiagnosisRole = 12227,
                    OrganizationId = 1,
                    PatientId = patientId,
                    Status = EOCStatus.Active,
                    Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate }
                };
            }

            return episodeOfCareDAL.InsertOrUpdate(eoc, user);
        }

        private int InsertEncounter(int episodeOfCareId)
        {
            Encounter encounterEntity = new Encounter()
            {
                Class = 12246,
                Period = new Domain.Sql.Entities.Common.PeriodDatetime()
                {
                    Start = DateTime.Now,
                    End = DateTime.Now
                },
                Status = 12218,
                EpisodeOfCareId = episodeOfCareId,
                Type = 12208,
                ServiceType = 11087
            };

            return encounterDAL.Insert(encounterEntity);
        }
    }
}
