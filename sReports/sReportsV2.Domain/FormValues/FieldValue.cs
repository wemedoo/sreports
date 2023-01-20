using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Form;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.FormValues
{
    public class FieldValue
    {
        public string Id { get; set; }
        public int ThesaurusId { get; set; }
        public List<string> Value { get; set; }
        [BsonRequired]
        public string InstanceId{ get; set; }
        public string Type { get; set; }
        public List<FormFieldValue> Options { get; set; }
        public List<string> ValueLabel { get; set; }

        public void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.Value = this.Value ?? new List<string>();
                this.Value.Add(value);
            }
        }

        public List<string> TryGetAllDifferentValues()
        {   
            List<string> result = new List<string>();
            if (Value != null && Value.Count > 0)
            {
                result = result.Distinct().ToList();
            }
            return result;
        }
    } 
}
