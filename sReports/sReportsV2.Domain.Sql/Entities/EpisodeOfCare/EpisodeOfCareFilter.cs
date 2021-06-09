using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCareFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PatientId { get; set; }
        public bool FilterByIdentifier { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public DateTime? PeriodStartDate { get; set; }

        public DateTime? PeriodEndDate { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
    }
}
