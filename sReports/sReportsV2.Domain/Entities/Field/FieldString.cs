using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public class FieldString : Field
    {
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }

        public string GetReferrableRepetitiveValue()
        {
            string result = string.Empty;

            foreach (string repetititveValue in this.Value)
            {
                result += $"{repetititveValue} <br>";
            }

            return result;
        }

        

    }
}
