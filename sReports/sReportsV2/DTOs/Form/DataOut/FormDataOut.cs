using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormDataOut
    {
        public string Id { get; set; }
        public FormAboutDataOut About { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public UserDataDataOut User { get; set; }
        public OrganizationDataOut Organization { get; set; }
        public List<FormChapterDataOut> Chapters { get; set; } = new List<FormChapterDataOut>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }
        public string ThesaurusId { get; set; }
        public DocumentPropertiesDataOut DocumentProperties { get; set; }
        public FormEpisodeOfCareDataDataOut EpisodeOfCare { get; set; }
        public List<FormStatusDataOut> WorkflowHistory { get; set; }
        public List<ReferralInfoDTO> ReferrableFields { get; set; }
        public bool DisablePatientData { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public FormDataOut()
        {
            WorkflowHistory = new List<FormStatusDataOut>();
        }
        public List<FieldDataOut> GetAllFields()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list =>
                                    list.SelectMany(set => set.Fields
                                    )
                                )
                            )
                        )
                        .ToList();
        }

        public List<List<FormFieldSetDataOut>> GetAllListOfFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        )
                        .ToList();
        }

        public void SetDependables()
        {
            List<FieldDataOut> fields = this.GetAllFields();

            foreach (FieldSelectableDataOut formFieldDataOut in this.GetAllFields().OfType<FieldSelectableDataOut>())
            {
                formFieldDataOut.GetDependablesData(fields, formFieldDataOut.Dependables);
            }
        }

        public void SetFields(List<FieldValue> fields, List<FormChapterDataOut> chapters) 
        {
            fields = Ensure.IsNotNull(fields, nameof(fields));
            chapters = Ensure.IsNotNull(chapters, nameof(chapters));

            this.Chapters = chapters;
            List<string> keys = fields?.Select(x => x.InstanceId).ToList();

            foreach (var list in this.GetAllListOfFieldSets())
            {
                List<string> fieldSetKeys = keys.Where(x => x.StartsWith($"{list[0].Id}-")).ToList();
                List<string> distinctedList = GetDistinctedList(fieldSetKeys);
                for (int i = 1; i < distinctedList.Count; i++) 
                {
                    list.Add(list[0].Clone());
                }

                foreach (string key in fieldSetKeys) 
                {
                    list[Int32.Parse(key.Split('-')[1])].Fields.FirstOrDefault(x => x.Id == key.Split('-')[2]).Value = fields.FirstOrDefault(x => x.InstanceId == key).Value;
                }
            }
        }

        private List<string> GetDistinctedList(List<string> allKeysForFieldSetId)
        {
            List<string> listForDistinct = new List<string>();

            foreach (string value in allKeysForFieldSetId)
            {
                listForDistinct.Add(value.Split('-')[1]);
            }

            return listForDistinct.Distinct().ToList();
        }
    }
}