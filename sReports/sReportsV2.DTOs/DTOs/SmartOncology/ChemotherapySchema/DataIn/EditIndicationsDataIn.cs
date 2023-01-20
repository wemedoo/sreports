using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class EditIndicationsDataIn
    {
        public int ChemotherapySchemaId { get; set; }
        public List<IndicationDataIn> Indications { get; set; }
        public string RowVersion { get; set; }
    }
}
