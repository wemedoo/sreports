using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class ChemotherapySchemaDataIn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RowVersion { get; set; }
        public int LengthOfCycle { get; set; }
        public int NumOfCycles { get; set; }
        public bool AreCoursesLimited { get; set; }
        public int NumOfLimitedCourses { get; set; }

        public List<IndicationDataIn> Indications { get; set; }
        public List<LiteratureReferenceDataIn> LiteratureReferences { get; set; }
        public List<MedicationDataIn> Medications { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
