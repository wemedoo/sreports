using sReportsV2.DTOs.Autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class MedicationInstanceAutocompleteDataIn : AutocompleteDataIn
    {
        public int ChemotherapySchemaInstanceId { get; set; }
        public bool IsSupportiveMedication { get; set; }
    }
}
