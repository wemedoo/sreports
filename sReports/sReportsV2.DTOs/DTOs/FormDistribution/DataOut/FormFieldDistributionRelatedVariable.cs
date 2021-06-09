using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormFieldDistributionRelatedVariable
    {
        public string Id { get; set; }
        public string VariableType { get; set; }
        public float? LowerBoundary { get; set; }
        public float? UpperBoundary { get; set; }

        public List<FormFieldValueDistributionDataOut> Values { get; set;  }

        public int GetNumericOptionsCount()
        {
            return LowerBoundary != null && UpperBoundary != null ? 3 : 2;
        }

        public string GetNumericLable(int current)
        {
            if(current == 0)
            {
                return $">{LowerBoundary ?? UpperBoundary}";
            }else if(current == 1)
            {
                if(LowerBoundary != null && UpperBoundary != null)
                {
                    return $"< {LowerBoundary} and <{UpperBoundary}";
                }
                else
                {
                    return $">{LowerBoundary ?? UpperBoundary}";
                }
            }else
            {
                return $"{UpperBoundary}<";
            }
        }
    }
}