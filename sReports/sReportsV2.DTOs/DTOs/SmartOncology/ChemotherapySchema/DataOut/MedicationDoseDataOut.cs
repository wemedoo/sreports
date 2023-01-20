using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class MedicationDoseDataOut
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public string StartsAt { get; set; }
        public List<MedicationDoseTimeDataOut> MedicationDoseTimes { get; set; } = new List<MedicationDoseTimeDataOut>();
        public int? IntervalId { get; set; }
        public int? UnitId { get; set; }
        public UnitDTO Unit { get; set; }
        public int MedicationId { get; set; }

        public string ToJson()
        {
            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(this));
        }
    }
}
