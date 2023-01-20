using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using System;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataOut
{
    public class FormInstanceStatusDataOut : FormStatusAbstractDataOut
    {
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByActiveOrganization { get; set; }
        public DateTime CreatedOn { get; set; }
        public FormState FormInstanceStatus { get; set; }
        public bool IsSigned { get; set; }

        public override dynamic StatusValue
        {
            get
            {
                return FormInstanceStatus;
            }
        }

        public override DateTime CreatedDateTime
        {
            get
            {
                return CreatedOn;
            }
        }

        public override string CreatedName
        {
            get
            {
                return CreatedByName;
            }
        }
    }
}
