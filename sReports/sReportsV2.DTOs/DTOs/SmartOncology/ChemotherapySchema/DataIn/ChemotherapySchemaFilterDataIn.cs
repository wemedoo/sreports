using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class ChemotherapySchemaFilterDataIn : Common.DataIn
    {
        public string Indication { get; set;}
        public string State { get; set; }
        public string ClinicalConstelation { get; set; }
        public string Name { get; set; }

    }
}
