using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    [BsonIgnoreExtraElements]
    public class FormFieldDistribution
    {   
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormFieldValueDistribution> Values { get; set; }
        public List<RelatedVariable> RelatedVariables { get; set; }
        public List<FormFieldDistributionSingleParameter> ValuesAll { get; set; }

        public RelatedVariable GetRelatedVariableById(string fieldId)
        {
            return this.RelatedVariables.FirstOrDefault(x => x.Id.Equals(fieldId));
        }
    }
}
