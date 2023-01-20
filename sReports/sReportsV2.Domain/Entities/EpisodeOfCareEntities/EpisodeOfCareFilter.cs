using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.EpisodeOfCareEntities
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class EpisodeOfCareFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string PatientId { get; set; }
        public bool FilterByIdentifier{get;set;}
        public string Status { get; set; }
        public string Type { get; set; }

        public DateTime? PeriodStartDate { get; set; }

        public DateTime? PeriodEndDate { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
    }
}
