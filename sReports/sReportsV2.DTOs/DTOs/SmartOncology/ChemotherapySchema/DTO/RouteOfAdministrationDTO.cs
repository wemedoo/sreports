using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO
{
    public class RouteOfAdministrationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string ShortName { get; set; }
        public string FDACode { get; set; }
        public string NCICondeptId { get; set; }
    }
}
