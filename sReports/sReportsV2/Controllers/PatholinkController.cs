using sReportsV2.Common.Enums;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReports.PathoLink.Entities;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.CustomAttributes;

namespace sReportsV2.Controllers
{
    public class PatholinkController : BaseController
    {
        private readonly IFormInstanceDAL formInstanceService;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormDAL formService;
        private readonly IThesaurusDAL thesaurusDAL;


        public PatholinkController(IThesaurusDAL thesaurusDAL, IEncounterDAL encounterDAL, IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL)
        {
            formInstanceService = new FormInstanceDAL();
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.patientDAL = patientDAL;
            this.encounterDAL = encounterDAL;
            this.thesaurusDAL = thesaurusDAL;
            formService = new FormDAL();
        }

        [HttpPost]
        public ActionResult Import([System.Web.Http.FromBody]PathoLink pathoLink)
        {
            if (string.IsNullOrEmpty(pathoLink.CaseDetails.submissionID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Submission id must be set");
            }

            FormInstance formInstance = formInstanceService.GetById(pathoLink.CaseDetails.submissionID);
            
            if(formInstance == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Form form = formService.GetForm(formInstance.FormDefinitionId);

            List<PathoLinkField> fields = pathoLink.ClinicalInformation.Concat<PathoLinkField>(pathoLink.Result).ToList();
            NormalizePathlinkFields(fields);
            
            foreach(FieldValue fv in formInstance.Fields)
            {
                switch (fv.Type)
                {
                    case FieldTypes.Calculative:
                    case FieldTypes.Date:
                    case FieldTypes.Datetime:
                    case FieldTypes.Digits:
                    case FieldTypes.Email:
                    case FieldTypes.LongText:
                    case FieldTypes.Number:
                    case FieldTypes.Regex:
                    case FieldTypes.Text:
                    case FieldTypes.URL:
                        fv.Value = GetStringFieldValueFromPathoLink(fields, fv.Id);
                        break;

                    case FieldTypes.Checkbox:
                    case FieldTypes.Select:
                    case FieldTypes.Radio:
                        fv.Value = GetSelectableFieldValueFromPathoLink(form.GetAllFields(), fields, fv);
                        break;
                }
            }

            formInstance.Id = null;
            formInstanceService.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult Export(string formInstanceId)
        {
            List<PathoLinkField> dataOutFields = new List<PathoLinkField>();
            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId));
            form.Id = formInstance.Id;
            Patient patient = GetPatient(formInstance);
            
            if (formInstanceId != null)            
            {
                List<ThesaurusEntry> thesauruses = thesaurusDAL.GetByIdsList(form.GetAllThesaurusIds());
                List<Field> fields = form.GetAllFields().ToList();
                MapFieldsToPathoLink(form, fields, dataOutFields, thesauruses);
            }

            PathoLink result = new PathoLink
            {
                CaseDetails = new CaseDetails()
                {
                    birthday = patient != null && patient.BirthDate.HasValue ? patient.BirthDate.Value.ToString(ViewBag.DateFormat) : "",
                    dateOfSurgery = formInstance.Date.HasValue ? formInstance.Date.Value.ToString(ViewBag.DateFormat) : "",
                    gender = patient == null ? "" : patient.Gender == Gender.Male ? "M" : patient.Gender == Gender.Female ? "F" : "",
                    submissionID = formInstance.Id,
                    
                },
                ClinicalInformation = dataOutFields
            };
            SetCustomResponseHeaderForMultiFileDownload();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<string> GetSelectableFieldValueFromPathoLink(List<Field> fields, List<PathoLinkField> patholinkFields, FieldValue fieldValue)
        {
            List<string> result = new List<string>();
            List<FormFieldValue> values = (fields.FirstOrDefault(x => x.Id.Equals(fieldValue.Id)) as FieldSelectable).Values;

            foreach (FormFieldValue value in values)
            {
                PathoLinkField patholinkField = patholinkFields.FirstOrDefault(x => x.o40MtId.Equals($"{fieldValue.Id}-{value.ThesaurusId}"));
                if (patholinkField.value == "true")
                {
                    result.Add(fieldValue.Type == FieldTypes.Radio ? value.ThesaurusId.ToString() : value.Value);
                }
            }

            return new List<string>() { string.Join(",", result) };
        }

        private List<string> GetStringFieldValueFromPathoLink(List<PathoLinkField> fields, string id)
        {
            return new List<string>() { fields.FirstOrDefault(x => x.o40MtId.Equals(id))?.value };
        }

        private void NormalizePathlinkFields(List<PathoLinkField> fields)
        {
            foreach (PathoLinkField field in fields)
            {
                field.RemoveFormIdFromO4MTId();
            }
        }

        private Patient GetPatient(FormInstance formInstance)
        {
            Encounter encounter = encounterDAL.GetById(formInstance.EncounterRef);
            if (encounter == null) return null;
            EpisodeOfCare episodeOfCareEntity = episodeOfCareDAL.GetById(encounter.EpisodeOfCareId);
            
            return patientDAL.GetById(episodeOfCareEntity.PatientId);
        }

        private void MapFieldsToPathoLink(Form formInstance, List<Field> fields, List<PathoLinkField> dataOutFields, List<ThesaurusEntry> thesauruses, string namePrefix = "")
        {
            foreach (Field field in fields)
            {
                ParseToPathoLinkField(field, formInstance, dataOutFields,thesauruses, namePrefix);
            }
        }

        private void ParseToPathoLinkField(Field field, Form formInstance, List<PathoLinkField> dataOutFields, List<ThesaurusEntry> thesauruses, string namePrefix)
        {
            if (field is FieldSelectable selectable)
            {
                InsertSelectableField(formInstance, selectable, dataOutFields,thesauruses, namePrefix);
            }
            else
            {
                InsertNonSelectableField(formInstance.Id, field, dataOutFields,thesauruses, namePrefix);
            }
        }

        private void InsertNonSelectableField(string formInstanceId, Field field, List<PathoLinkField> dataOutFields, List<ThesaurusEntry> thesauruses, string namePrefix)
        {
            string o40mtid = $"{formInstanceId}-{field.Id}";
            PathoLinkField pathLinkField = new PathoLinkField
            {
                value = field.GetPatholinkValue(Resources.TextLanguage.N_E),
                name = string.IsNullOrWhiteSpace(namePrefix) ? $"[{field.Label}] [{field.Label}]" : $"{namePrefix} [{field.Label}]",
                defaultValue = string.Empty,
                type = field.Type,
                o40MtId = o40mtid
            };

            if (!dataOutFields.Any(x => x.o40MtId.Equals(o40mtid)))
            {
                dataOutFields.Add(pathLinkField);
            }
        }

        private void InsertSelectableField(Form formInstance, FieldSelectable field, List<PathoLinkField> dataOutFields, List<ThesaurusEntry> thesauruses, string namePrefix)
        {
            field.Values.ForEach(x =>
            {
                string name = string.IsNullOrWhiteSpace(namePrefix) ? $"[{field.Label}] [{x.Value}]" : $"{namePrefix} [{field.Label}] [{x.Value}]";
                string o40mtid = $"{formInstance.Id}-{field.Id}-{x.ThesaurusId}";
                PathoLinkField pathLinkField = new PathoLinkField
                {
                    value = field.GetPatholinkValue(Resources.TextLanguage.N_E, field.Type == FieldTypes.Radio ? x.ThesaurusId.ToString() : x.Value),
                    name = name,
                    defaultValue = "false",
                    type = field.Type,
                    o40MtId = o40mtid
                };

                if (!dataOutFields.Any(y => y.o40MtId.Equals(o40mtid)))
                {
                    dataOutFields.Add(pathLinkField);
                }
                InsertDependableField(field.Dependables.Where(y => y.Condition.Contains(x.Value)), formInstance, dataOutFields, name, thesauruses);
            });
        }

        private void InsertDependableField(IEnumerable<FormFieldDependable> dependables, Form formInstance, List<PathoLinkField> dataOutFields, string namePrefix, List<ThesaurusEntry> thesauruses)
        {
            IEnumerable<Field> dependableFields = formInstance.GetAllFieldsWhichAreDependable();
            foreach (FormFieldDependable dependable in dependables)
            {
                List<Field> fields = dependableFields.Where(df => dependable.ActionParams.Contains(df.Id)).ToList();
                MapFieldsToPathoLink(formInstance, fields, dataOutFields,thesauruses, namePrefix);
            }
        }
    }
}