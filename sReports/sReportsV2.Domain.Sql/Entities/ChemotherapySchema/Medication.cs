using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class Medication : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationId")]
        public int MedicationId { get; set; }
        public string Name { get; set; }
        public string Dose { get; set; }
        public string PreparationInstruction { get; set; }
        public string ApplicationInstruction { get; set; }
        public string AdditionalInstruction { get; set; }
        public int RouteOfAdministration { get; set; }
        public int BodySurfaceCalculationFormula { get; set; }

        public bool SameDoseForEveryAplication { get; set; }
        public bool HasMaximalCumulativeDose { get; set; }
        public string CumulativeDose { get; set; }

        public bool WeekendHolidaysExcluded { get; set; }
        public int? MaxDayNumberOfApplicationiDelay { get; set; }

        public bool IsSupportiveMedication { get; set; }
        public bool SupportiveMedicationReserve { get; set; }
        public bool SupportiveMedicationAlternative { get; set; }
        public int? ChemotherapySchemaId { get; set; }
        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        public int? CumulativeDoseUnitId { get; set; }
        [ForeignKey("CumulativeDoseUnitId")]
        public virtual Unit CumulativeDoseUnit { get; set; }

        public List<MedicationDose> MedicationDoses { get; set; } = new List<MedicationDose>();

        public List<MedicationDose> GetPremedicationsDays()
        {
            return MedicationDoses.Where(m => !m.IsDeleted && m.DayNumber < 0).ToList();
        }

        public List<MedicationDose> GetMedicationDays()
        {
            return MedicationDoses.Where(m => !m.IsDeleted && m.DayNumber > 0).ToList();
        }

        public void Copy(Medication medication)
        {
            this.MedicationId = medication.MedicationId;
            this.Name = medication.Name;
            this.Dose = medication.Dose;
            this.UnitId = medication.UnitId;
            this.PreparationInstruction = medication.PreparationInstruction;
            this.ApplicationInstruction = medication.ApplicationInstruction;
            this.AdditionalInstruction = medication.AdditionalInstruction;
            this.RouteOfAdministration = medication.RouteOfAdministration;
            this.BodySurfaceCalculationFormula = medication.BodySurfaceCalculationFormula;
            this.SameDoseForEveryAplication = medication.SameDoseForEveryAplication;
            this.HasMaximalCumulativeDose = medication.HasMaximalCumulativeDose;
            this.CumulativeDose = medication.CumulativeDose;
            this.CumulativeDoseUnitId = medication.CumulativeDoseUnitId;
            this.WeekendHolidaysExcluded = medication.WeekendHolidaysExcluded;
            this.MaxDayNumberOfApplicationiDelay = medication.MaxDayNumberOfApplicationiDelay;
            this.IsSupportiveMedication = medication.IsSupportiveMedication;
            this.SupportiveMedicationReserve = medication.SupportiveMedicationReserve;
            this.SupportiveMedicationAlternative = medication.SupportiveMedicationAlternative;
            this.ChemotherapySchemaId = medication.ChemotherapySchemaId;

            CopyRowVersion(medication.RowVersion);
            UpdateMedicationDoses(medication.MedicationDoses);
        }

        public void CopyRowVersion(byte[] rowVersion)
        {
            this.RowVersion = rowVersion;
        }

        public void UpdateMedicationDoses(List<MedicationDose> newMedicationDoses)
        {
            DeleteExistingRemovedDoses(newMedicationDoses);
            AddNewOrUpdateOldDoses(newMedicationDoses);
        }

        private void DeleteExistingRemovedDoses(List<MedicationDose> upcomingDoses)
        {
            foreach (var medicationDose in MedicationDoses)
            {
                var remainingDose = upcomingDoses.Any(x => x.MedicationDoseId == medicationDose.MedicationDoseId);
                if (!remainingDose)
                {
                    medicationDose.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldDoses(List<MedicationDose> upcomingDoses)
        {
            foreach (var dose in upcomingDoses)
            {
                if (dose.MedicationDoseId == 0)
                {
                    MedicationDoses.Add(dose);
                }
                else
                {
                    var dbDose = MedicationDoses.FirstOrDefault(x => x.MedicationDoseId == dose.MedicationDoseId && !x.IsDeleted);
                    if (dbDose != null)
                    {
                        dbDose.Copy(dose);
                    }
                }
            }
        }
    }
}
