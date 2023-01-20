using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.Common.DTO
{
    public class ResourceCreatedDTO
    {
        public string Id { get; set; }
        public string RowVersion { get; set; }
        public string LastUpdate { get; set; }
    }
}
