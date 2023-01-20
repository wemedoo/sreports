using sReportsV2.Common.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class ChemotherapySchemaDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int LengthOfCycle { get; set; }
        public int NumOfCycles { get; set; }
        public bool AreCoursesLimited { get; set; }
        public int NumOfLimitedCourses { get; set; }

        public List<IndicationDataOut> Indications { get; set; }
        public List<LiteratureReferenceDataOut> LiteratureReferences { get; set; }
        public List<MedicationPreviewDataOut> Medications { get; set; }
        public UserData Creator { get; set; }
        public string RowVersion { get; set; }
        public int GetNumberOfWeeks()
        {
            return LengthOfCycle / 7;
        }

        public int GetNumberOfDays()
        {
            return LengthOfCycle % 7;
        }

        public List<MedicationPreviewDataOut> FilterMedications(bool isSupportiveTherapy = false)
        {
            return Medications.Where(m => m.IsSupportiveMedication == isSupportiveTherapy).ToList();
        }
    }
}
