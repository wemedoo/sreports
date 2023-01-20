using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DigitalGuideline.DataOut.EvidenceProperties;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;


namespace sReportsV2.DTOs.DigitalGuideline.DataOut
{
    public class GuidelineElementDataDataOut
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public NodeState State { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public string Title { get; set; }
        public virtual string Type { get; set; }
        public EvidencePropertiesDataOut EvidenceProperties { get; set; }

        public object ToJson()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this, serializerSettings));
        }
    }
}