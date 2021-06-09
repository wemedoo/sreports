using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataOut
{
    public class GuidelineDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public DateTime? EntryDateTime { get; set; }

        public DateTime? LastUpdate { get; set; }

        public GuidelineElementsDataOut GuidelineElements { get; set; }
        public VersionDTO Version { get; set; }


        public JObject ToJsonNodeElements()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            JObject result = new JObject(
                new JProperty("nodes", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GuidelineElements?.Nodes ?? null, serializerSettings))), 
                new JProperty("edges", JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GuidelineElements?.Edges ?? null, serializerSettings)))
            );

            return result;
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
    }

    public class GuidelineElementsDataOut
    {
        public List<GuidelineElementDataOut> Nodes { get; set; }
        public List<GuidelineElementDataOut> Edges { get; set; }
    }
}