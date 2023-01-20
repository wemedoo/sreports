using sReportsV2.Common.SmartOncologyEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaDayDataOut
    {
        public int DayNumber { get; set; }
        public string Signature { get; set; }

        public List<SchemaMedicationDataOut> CancerDirectedTreatment { get; set; } = new List<SchemaMedicationDataOut>();
        public List<SchemaMedicationDataOut> SupportiveTherapy { get; set; } = new List<SchemaMedicationDataOut>();

        public string LaboratoryData { get; set; }
        public string FindingsData { get; set; }
        public string SymptomsData { get; set; }
        public string TumorResponse { get; set; }
        public string TumorMarkers { get; set; }

        public void AddMedication(SchemaMedicationDataOut medicationData, List<SchemaMedicationDataOut> treatments)
        {
            treatments.Add(medicationData);
        }

        public string GetDose(string medicationName, TreatmentType treatmentTherapy)
        {
            SchemaMedicationDataOut medicationData = GetTreatmentByType(treatmentTherapy).FirstOrDefault(t => t.Name == medicationName);
            if (medicationData != null)
            {
                return medicationData.Dose.ToString();
            }
            else
            {
                return "";
            }
        }

        public List<SchemaMedicationDataOut> GetTreatmentByType(TreatmentType treatmentTherapy)
        {
            switch (treatmentTherapy)
            {
                case TreatmentType.CancerDirectedTreatment:
                    return CancerDirectedTreatment;
                case TreatmentType.SupportiveTherapy:
                default:
                    return SupportiveTherapy;
            }

        }
    }
}