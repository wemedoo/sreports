using Newtonsoft.Json;
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
            formInstanceService.InsertOrUpdate(formInstance);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
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
            foreach(PathoLinkField field in fields)
            {
                field.RemoveFormIdFromO4MTId();
            }
        }

        // GET: Patholink
        public ActionResult Export(string formInstanceId)
        {
            List<PathoLinkField> dataOutFields = new List<PathoLinkField>();
            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId));
            form.Id = formInstance.Id;
            Encounter encounter = encounterDAL.GetById(formInstance.EncounterRef);
            EpisodeOfCare episodeOfCareEntity = episodeOfCareDAL.GetById(encounter.EpisodeOfCareId);
            Patient patientEntity = patientDAL.GetById(episodeOfCareEntity.PatientId);
            if(formInstanceId != null)            
            {
                List<ThesaurusEntry> thesauruses = thesaurusDAL.GetByIdsList(form.GetAllThesaurusIds());
                List<Field> fields = form.GetAllFields().ToList();
                MapFieldsToPathoLink(form, fields, dataOutFields, thesauruses);
            }

            PathoLink result = new PathoLink
            {
                CaseDetails = new CaseDetails()
                {
                    birthday = patientEntity.BirthDate != null ? patientEntity.BirthDate.Value.ToShortDateString() : "",
                    dateOfSurgery = formInstance.Date.ToString(),
                    gender = patientEntity.Gender == Gender.Male ? "M" : patientEntity.Gender == Gender.Female ? "F" : "",
                    submissionID = formInstance.Id,
                    
                },
                ClinicalInformation = dataOutFields
            };
            return Json(result, JsonRequestBehavior.AllowGet);
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
            if (field.Type.Equals(FieldTypes.Radio) || field.Type == FieldTypes.Checkbox || field.Type == FieldTypes.Select)
            {
                InsertSelectableField(formInstance, (FieldSelectable)field, dataOutFields,thesauruses, namePrefix);
            }
            else
            {
                InsertNonSelectableField(formInstance.Id, (FieldString)field, dataOutFields,thesauruses, namePrefix);
            }
        }

        private void InsertNonSelectableField(string formInstanceId, FieldString field, List<PathoLinkField> dataOutFields, List<ThesaurusEntry> thesauruses, string namePrefix)
        {
            string id = thesauruses.FirstOrDefault(x => x.Id.Equals(field.ThesaurusId))?.Codes?.FirstOrDefault(z => z.System.Equals("Patholink"))?.Code;
            string o40mtid = $"{formInstanceId}-{field.Id}";
            PathoLinkField pathLinkField = new PathoLinkField
            {
                value = field.GetValue(),
                id = id,
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
                string id = thesauruses.FirstOrDefault(y => y.Id.ToString().Equals(x.ThesaurusId))?.Codes?.FirstOrDefault(z => z.System.Equals("Patholink"))?.Code;

                string name = string.IsNullOrWhiteSpace(namePrefix) ? $"[{field.Label}] [{x.Value}]" : $"{namePrefix} [{field.Label}] [{x.Value}]";
                string o40mtid = $"{formInstance.Id}-{field.Id}-{x.ThesaurusId}";
                PathoLinkField pathLinkField = new PathoLinkField
                {
                    value = field.Value != null && field.Value.Count > 0 && field.Value[0].Contains(x.Value) ? "true" : "",
                    id = id,
                    name = name,
                    defaultValue = "false",
                    type = field.Type,
                    o40MtId = o40mtid
                };

                if (field.Type == FieldTypes.Radio) 
                {
                    pathLinkField.value = field.Value != null && field.Value.Count > 0 && field.Value[0].Contains(x.ThesaurusId.ToString()) ? "true" : "";
                }

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