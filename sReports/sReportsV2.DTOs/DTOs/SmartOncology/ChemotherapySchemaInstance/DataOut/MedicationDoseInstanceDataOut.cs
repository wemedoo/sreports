using Newtonsoft.Json;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class MedicationDoseInstanceDataOut
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public DateTime? Date { get; set; }
        public List<MedicationDoseTimeInstanceDataOut> MedicationDoseTimes { get; set; } = new List<MedicationDoseTimeInstanceDataOut>();
        public string StartsAt { get; set; }
        public int MedicationInstanceId { get; set; }
        public int? UnitId { get; set; }
        public UnitDTO Unit { get; set; }
        public string RowVersion { get; set; }
        public int? IntervalId { get; set; }

        public string ToJson()
        {
            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(this));
        }

        public string RenderTimeDosesValue()
        {
            var doseTimes = MedicationDoseTimes.Select(x => { return string.Concat(x.Time, "h : ", RenderDose(x.Dose)); });
            var unit = RenderUnit();
            return string.Format("{0} {1}", string.Join(";\n", doseTimes), string.IsNullOrEmpty(unit) ? string.Empty : $"[{unit}]");
        }

        public string RenderTimeDosesTooltip()
        {
            var doseTimes = MedicationDoseTimes.Select(x => { return string.Format("<li>{0}h : {1} {2}</li>", x.Time, RenderDose(x.Dose), RenderUnit()); });
            return string.Format("<div><h4>Dosing time:</h4><ol>{0}</ol></div>", string.Join("", doseTimes));
        }

        private string RenderUnit()
        {
            return UnitId.HasValue ? Unit.Name : string.Empty;
        }

        private string RenderDose(string dose)
        {
            return string.IsNullOrEmpty(dose) ? "N/A" : dose;
        }
    }
}
