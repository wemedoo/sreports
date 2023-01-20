using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientAddress : AddressBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PatientAddressId")]
        public int PatientAddressId { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        public PatientAddress() { }

        public PatientAddress(AddressBase address, int patientId)
        {
            Copy(address);
            PatientId = patientId;
        }
    }
}
