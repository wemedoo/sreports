using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormStatusDataOut
    {
        public FormDefinitionState Status { get; set; }
        public DateTime Created { get; set; }
        public UserDataOut User { get; set; }
        public string GetStatusColor(string status) 
        {
            string color;

            if (status == "Disabled")
                color = "rect-disabled";
            else if (status == "ReadyForDataCapture")
                color = "rect-dataCapture";
            else if (status == "ReadyForReview")
                color = "rect-ForReview";
            else
                color = "rect-onGoing";

            return color;
        }
    }
}