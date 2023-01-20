using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO
{
    public class ChemotherapySchemaResourceCreatedDTO : ResourceCreatedDTO
    {
        public int ParentId { get; set; }
    }
}
