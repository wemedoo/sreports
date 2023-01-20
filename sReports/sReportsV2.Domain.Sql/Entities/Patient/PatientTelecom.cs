using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientTelecom : TelecomBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientTelecomId")]
        public int PatientTelecomId { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }
        public PatientTelecom() { }

        public PatientTelecom(TelecomBase telecom, int patientId)
        {
            Copy(telecom);
            PatientId = patientId;
        }
    }
}
