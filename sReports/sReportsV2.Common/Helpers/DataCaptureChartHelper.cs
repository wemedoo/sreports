using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Common.Helpers
{
    public class DataCaptureChartUtility
    {
        public Dictionary<string, DateValueList> labelValuesDict;

        public DataCaptureChartUtility()
        {
            labelValuesDict = new Dictionary<string, DateValueList>();
        }

        public void AddToKeyIfExists(string label, double? value, long dateInMilliseconds)
        {
            if (labelValuesDict.ContainsKey(label))
            {
                labelValuesDict[label].Add(value, dateInMilliseconds);
            }
            else
            {
                labelValuesDict.Add(label, new DateValueList().Add(value, dateInMilliseconds));
            }
        }
    }
    
    public class DateValueList
    {
        public DateValueList Add(double? value, long dateInMilliseconds)
        {
            Value.Add(value);
            Date.Add(dateInMilliseconds);
            return this;
        }
        public List<double?> Value { get; set; } = new List<double?>();
        public List<double> Date { get; set; } = new List<double>();
    }
}