using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class ChemotherapySchemaInstance : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ChemotherapySchemaInstanceId")]
        public int ChemotherapySchemaInstanceId { get; set; }
        public DateTime? StartDate { get; set; }
        public List<MedicationInstance> Medications { get; set; } = new List<MedicationInstance>();
        [ForeignKey("CreatorId")]
        public virtual sReportsV2.Domain.Sql.Entities.User.User Creator { get; set; }
        [ForeignKey("ChemotherapySchemaId")]
        public virtual sReportsV2.Domain.Sql.Entities.ChemotherapySchema.ChemotherapySchema ChemotherapySchema { get; set; }
        [ForeignKey("PatientId")]
        public virtual SmartOncologyPatient.SmartOncologyPatient Patient { get; set; }
        public int PatientId { get; set; }
        public int CreatorId { get; set; }
        public int ChemotherapySchemaId { get; set; }
        public InstanceState State { get; set; }
        public List<ChemotherapySchemaInstanceVersion> ChemotherapySchemaInstanceHistory { get; set; } = new List<ChemotherapySchemaInstanceVersion>();
        public List<MedicationReplacement> MedicationReplacements { get; set; } = new List<MedicationReplacement>();
        public void Copy(ChemotherapySchemaInstance chemotherapySchemaInstance)
        {
            this.RowVersion = chemotherapySchemaInstance.RowVersion;
            this.StartDate = chemotherapySchemaInstance.StartDate;

            CopyMedications(chemotherapySchemaInstance.Medications);
        }

        public void CopyMedications(List<MedicationInstance> medications)
        {
            foreach(var medication in Medications)
            {
                MedicationInstance updatedMedication = medications.FirstOrDefault(m => m.MedicationId == medication.MedicationId);
                if(updatedMedication != null)
                {
                    medication.Copy(updatedMedication);
                }
            }
        }
    }
}
