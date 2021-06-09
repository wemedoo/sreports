using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
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
        public static string DefaultIdPlaceholder { get; set; } = "formIdPlaceHolder";
        public static string DefaultFormPlaceholder { get; set; } = "Form title";

        private string _id;
        [DataProp]
        public string Id 
        { 
            get {return string.IsNullOrWhiteSpace(_id) ? DefaultIdPlaceholder : _id;}
            set {_id = value;} 
        }
        [DataProp]
        public FormAboutDataOut About { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        [DataProp]
        public DateTime? EntryDatetime { get; set; }
        [DataProp]
        public DateTime? LastUpdate { get; set; }
        [DataProp]
        public UserDataOut User { get; set; }
        [DataProp]
        public OrganizationDataOut Organization { get; set; }
        public List<FormChapterDataOut> Chapters { get; set; } = new List<FormChapterDataOut>();
        [DataProp]
        public FormDefinitionState State { get; set; }
        [DataProp]
        public string Language { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public DocumentPropertiesDataOut DocumentProperties { get; set; }
        [DataProp]
        public FormEpisodeOfCareDataDataOut EpisodeOfCare { get; set; }
        public List<FormStatusDataOut> WorkflowHistory { get; set; }
        public List<ReferralInfoDTO> ReferrableFields { get; set; }
        [DataProp]
        public bool DisablePatientData { get; set; }
        public string Description { get; set; }

        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }

        public bool IsParameterize{get;set;}
        public FormDataOut()
        {
            WorkflowHistory = new List<FormStatusDataOut>();
        }

        public object ToJson()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this, serializerSettings));
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

        public string GetActiveVersionJsonString()
        {
            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(Version, Formatting.None));
        }

        public static string GetInitialDataAttributes()
        {
            return $"data-title='New Form' data-id='{DefaultIdPlaceholder}'";
        }

        public string GetStateColor(FormDefinitionState status)
        {
            string color = "";
            switch (status)
            {
                case FormDefinitionState.DesignPending:
                    color = "#f7af00";
                    break;
                case FormDefinitionState.Design:
                    color = "#ffa500";
                    break;
                case FormDefinitionState.ReviewPending:
                    color = "#FF0000";
                    break;
                case FormDefinitionState.Review:
                    color = "#aced16";
                    break;
                case FormDefinitionState.ReadyForDataCapture:
                    color = "#daf00d";
                    break;
                case FormDefinitionState.Archive:
                    color = "#bdc6c7";
                    break;
            }
            return color;
        }


    }
}