using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.Value = this.Value ?? new List<string>();
                this.Value.Add(value);
            }
        }
    } 
}
