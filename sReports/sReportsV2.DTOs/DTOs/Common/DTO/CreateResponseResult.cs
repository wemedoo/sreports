using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class CreateResponseResult
    {
        public int Id { get; set; }
        public string LastUpdate { get; set; }
        public byte[] RowVersion { get; set; }
        public string Message { get; set; }
    }
}