using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceStatus
    {
        public FormState Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public bool IsSigned { get; set; }

        public FormInstanceStatus(FormState formInstanceStatus, int createdById, bool isSigned)
        {
            Status = formInstanceStatus;
            CreatedById = createdById;
            IsSigned = isSigned;
            CreatedOn = DateTime.Now;
        }
    }
}
