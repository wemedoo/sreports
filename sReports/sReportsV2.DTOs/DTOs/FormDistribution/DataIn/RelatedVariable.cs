using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class RelatedVariable
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public float? UpperBoundary { get; set; }
        public float? LowerBoundary { get; set; }

        public List<string> GetOptions()
        {
            List<string> result = new List<string>();
            if (UpperBoundary != null && LowerBoundary != null)
            {
                result.Add("LTE");
                result.Add("BTW");
                result.Add("GT");
            }else if(UpperBoundary != null || LowerBoundary != null)
            {
                result.Add("LTE");
                result.Add("GT");
            }
            return result;
        }

        public string GetLabelForOption(string option)
        {

            string result = string.Empty;
            switch (option)
            {
                case "LTE":
                    result = $" < {LowerBoundary ?? UpperBoundary}";
                    break;
                case "BTW":
                    result = $">{LowerBoundary} and < {UpperBoundary}";
                    break;
                case "GT":
                    result = $">{UpperBoundary ?? LowerBoundary}";
                    break;
            }
            return result;
        }

    }
}