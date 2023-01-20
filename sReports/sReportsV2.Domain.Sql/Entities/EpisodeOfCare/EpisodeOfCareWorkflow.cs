using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCareWorkflow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("EpisodeOfCareWorkflowId")]
        public int EpisodeOfCareWorkflowId { get; set; }
        public string DiagnosisCondition { get; set; }
        public int DiagnosisRole { get; set; }
        public int User { get; set; }
        public DateTime Submited { get; set; }
        public EOCStatus Status { get; set; }
        [ForeignKey("EpisodeOfCareId")]
        public EpisodeOfCare EpisodeOfCare { get; set; }
        public int EpisodeOfCareId { get; set; }
    }
}
