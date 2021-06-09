using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCareWorkflow
    {
        public int Id { get; set; }
        public string DiagnosisCondition { get; set; }
        public int DiagnosisRole { get; set; }
        public int User { get; set; }
        public DateTime Submited { get; set; }
        public EOCStatus Status { get; set; }
        public EpisodeOfCare EpisodeOfCare { get; set; }
        public int EpisodeOfCareId { get; set; }
    }
}
