using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Autocomplete
{
    public class AutocompleteDataIn
    {
        public string Term { get; set; }

        public string ExcludeId { get; set; }

        public int Page { get; set; }
    }
}