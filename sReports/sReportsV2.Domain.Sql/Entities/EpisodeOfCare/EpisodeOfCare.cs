using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.EpisodeOfCare
{
    public class EpisodeOfCare :EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("EpisodeOfCareId")]
        public int EpisodeOfCareId { get; set; }
        public int PatientId { get; set; }
        public int OrganizationId { get; set; }
        public EOCStatus Status { get; set; }
        public int Type { get; set; }
        public string DiagnosisCondition { get; set; }
        public int DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
        public PeriodDatetime Period { get; set; }
        public string Description { get; set; }
        public List<EpisodeOfCareWorkflow> WorkflowHistory { get; set; }
        [Column("SmartOncologyPatientId")]
        public int? SmartOncologyPatientId { get; set; }
      

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.DiagnosisRole = this.DiagnosisRole == oldThesaurus ? newThesaurus : this.DiagnosisRole;
            this.Type = this.Type == oldThesaurus ? newThesaurus : this.Type;
        }

        public void SetWorkflow(UserData user)
        {
            if (this.WorkflowHistory == null)
            {
                this.WorkflowHistory = new List<EpisodeOfCareWorkflow>();
            }

            this.WorkflowHistory.Add(
                    new EpisodeOfCareWorkflow()
                    {
                        DiagnosisCondition = this.DiagnosisCondition,
                        DiagnosisRole = this.DiagnosisRole,
                        User = user.Id,
                        Status = this.Status,
                        Submited = DateTime.Now
                    }
                );
        }
    }
}
