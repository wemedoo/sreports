using Newtonsoft.Json;
using sReports.PathoLink.Entities;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class PatholinkController : BaseController
    {
        private readonly IFormInstanceService formInstanceService;
        private readonly IEpisodeOfCareService episodeOfCareService;
        private readonly IPatientService patientService;
        private readonly IEncounterService encounterService;
        private readonly IThesaurusEntryService thesaurusService;
        private readonly IFormService formService;

        public PatholinkController()
        {
            formInstanceService = new FormInstanceService();
            episodeOfCareService = new EpisodeOfCareService();
            patientService = new PatientService();
            encounterService = new EncounterService();
            thesaurusService = new ThesaurusEntryService();
            formService = new FormService();
        }
        // GET: Patholink
        public ActionResult Export(string formInstanceId)
        {
            List<PathoLinkField> dataOutFields = new List<PathoLinkField>();
            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId));
            form.Id = formInstance.Id;
            EncounterEntity encounter = encounterService.GetById(formInstance.EncounterRef);
            EpisodeOfCareEntity episodeOfCareEntity = episodeOfCareService.GetEOCById(encounter.EpisodeOfCareId);
            PatientEntity patientEntity = patientService.GetById(episodeOfCareEntity.PatientId);
            if(formInstanceId != null)            
            {
                List<ThesaurusEntry> thesauruses = thesaurusService.GetByIdsList(form.GetAllThesauruses());
                List<Field> fields = form.GetAllFields().ToList();
                MapFieldsToPathoLink(form, fields, dataOutFields, thesauruses);
            }

            PathoLink result = new PathoLink
            {
                CaseDetails = new CaseDetails()
                {
                    birthday = patientEntity.BirthDate.ToString(),
                    dateOfSurgery = episodeOfCareEntity.Period.Start.ToString(),
                    gender = patientEntity.Gender == Domain.Enums.Gender.Male ? "M" : patientEntity.Gender == Domain.Enums.Gender.Female ? "F" : "",
                    submissionID = formInstance.Id                    
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
            string id = thesauruses.FirstOrDefault(x => x.O40MTId.Equals(field.ThesaurusId))?.Codes?.FirstOrDefault(z => z.System.Equals("Patholink"))?.Code;
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
                string id = thesauruses.FirstOrDefault(y => y.O40MTId.Equals(x.ThesaurusId))?.Codes?.FirstOrDefault(z => z.System.Equals("Patholink"))?.Code;

                string name = string.IsNullOrWhiteSpace(namePrefix) ? $"[{field.Label}] [{x.Value}]" : $"{namePrefix} [{field.Label}] [{x.Value}]";
                string o40mtid = $"{formInstance.Id}-{field.Id}-{x.ThesaurusId}";
                PathoLinkField pathLinkField = new PathoLinkField
                {
                    value = field.Value != null && field.Value.Contains(x.Value) ? "true" : "",
                    id = id,
                    name = name,
                    defaultValue = "false",
                    type = field.Type,
                    o40MtId = o40mtid
                };

                if (field.Type == "radio") 
                {
                    pathLinkField.value = field.Value != null && field.Value.Contains(x.ThesaurusId) ? "true" : "";
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