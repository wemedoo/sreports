using Hl7.Fhir.Model;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormFieldValue
    {
        public O4CodeableConcept Code { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string ThesaurusId { get; set; }
        public double? NumericValue { get; set; }

        public string GetValueToStore(string type) 
        {
            if(type == FieldTypes.Radio)
            {
                return this.ThesaurusId;
            }
            else
            {
                return this.Value;
            }
        }
    }
}
