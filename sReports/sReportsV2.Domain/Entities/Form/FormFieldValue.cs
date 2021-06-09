using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
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
        public string Id { get; set; }
        public O4CodeableConcept Code { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public double? NumericValue { get; set; }

        public string GetValueToStore(string type) 
        {
            if(type == FieldTypes.Radio)
            {
                return this.ThesaurusId.ToString();
            }
            else
            {
                return this.Value;
            }
        }
        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
        }
    }
}
