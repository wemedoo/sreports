using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Calculative)]
    public class FieldCalculative : Field
    {
        public override string Type { get; set; } = FieldTypes.Calculative;
        public string Formula { get; set; }

        public List<string> GetFormulaFields()
        {
            List<string> result = new List<string>();

            string[] spliited = Formula.Split('[');
            foreach (string split in spliited.Where(x => x.Contains("]")))
            {
                string fieldData = split.Trim();
                int indexOfBracket = fieldData.IndexOf("]");
                string fieldId = fieldData.Substring(0, indexOfBracket);
                result.Add(fieldId);
            }
            return result;
        }
    }
}
