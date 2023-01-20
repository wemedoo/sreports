using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO
{
    public class MedicationDoseTypeDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string StartAt { get; set; }
        public List<string> IntervalsList { get; set; }

    }
}
