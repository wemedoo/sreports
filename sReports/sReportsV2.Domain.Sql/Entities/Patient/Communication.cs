using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Communication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CommunicationId")]
        public int CommunicationId { get; set; }
        public string Language { get; set; }
        public bool Preferred { get; set; }
        [Column("PatientId")]
        public int? PatientId { get; set; }
        [Column("SmartOncologyPatientId")]
        public int? SmartOncologyPatientId { get; set; }
        public Communication() { }
        public Communication(string language, bool preferred)
        {
            this.Language = language;
            this.Preferred = preferred;
        }

        public void Copy(Communication communication)
        {
            this.Preferred = communication.Preferred;
        }
    }
}
