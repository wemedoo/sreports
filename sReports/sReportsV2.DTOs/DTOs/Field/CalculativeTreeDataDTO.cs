using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field
{
    public class CalculativeTreeDataDTO
    {
        public List<CalculativeTreeItemDTO> Data { get; set; }   
    }

    public class CalculativeTreeItemDTO
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string VariableName { get; set; }
    }
}