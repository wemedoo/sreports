using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class MedicationPreviewDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dose { get; set; }
        public string Unit { get; set; }
        public string PreparationInstruction { get; set; }
        public string ApplicationInstruction { get; set; }
        public string RouteOfAdministration { get; set; }
        public int ChemotherapySchemaId { get; set; }
        public bool IsSupportiveMedication { get; set; }
        public RouteOfAdministrationDTO RouteOfAdministrationDTO {get; set;}
        public List<MedicationDosePreviewDataOut> MedicationDoses { get; set; } = new List<MedicationDosePreviewDataOut>();
    }
}
