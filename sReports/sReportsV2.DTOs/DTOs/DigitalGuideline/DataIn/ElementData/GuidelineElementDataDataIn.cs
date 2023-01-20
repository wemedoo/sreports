using JsonSubTypes;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DigitalGuideline.DataIn.EvidenceProperties;
using sReportsV2.DTOs.DigitalGuideline.DTO;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(GuidelineDecisionElementDataDataIn), GuidelineElementDataTypes.Decision)]
    [JsonSubtypes.KnownSubType(typeof(GuidelineStatementElementDataDataIn), GuidelineElementDataTypes.Statement)]
    [JsonSubtypes.KnownSubType(typeof(GuidelineEdgeElementDataDataIn), GuidelineElementDataTypes.Edge)]
    [JsonSubtypes.KnownSubType(typeof(GuidelineEventElementDataDataIn), GuidelineElementDataTypes.Event)]
    public class GuidelineElementDataDataIn
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public virtual string Type { get; set; }
        public EvidencePropertiesDataIn EvidenceProperties { get; set; }
        public string Value { get; set; }
    }
}