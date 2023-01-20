using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class MedicationDataOut
    {
        public int Id { get; set; }
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
        public List<MedicationDoseDataOut> MedicationDoses { get; set; } = new List<MedicationDoseDataOut>();
        public int? ChemotherapySchemaId { get; set; }
        public int? UnitId { get; set; }
        public int? CumulativeDoseUnitId { get; set; }
        public string RowVersion { get; set; }

        public List<MedicationDoseDataOut> GetMedicationDoses(bool filterByPremedication = false)
        {
            if (filterByPremedication)
            {
                return MedicationDoses.Where(d => d.DayNumber <= 0).OrderByDescending(d => d.DayNumber).ToList();
            }
            else
            {
                return MedicationDoses.Where(d => d.DayNumber > 0).OrderBy(d => d.DayNumber).ToList();
            }
        }
    }
}
