using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn
{
    public class SmartOncologyPatientFilterDataIn : Common.DataIn
    {
        public string Name { get; set; }
        public int SelectedPatientId { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
