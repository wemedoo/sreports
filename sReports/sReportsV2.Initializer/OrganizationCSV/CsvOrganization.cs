using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Initializer.OrganizationCSV
{
    public class CsvOrganization
    {
        [Index(0)]
        public string InstitutionName { get; set; }
        [Index(1)]
        public string NumOfLocations { get; set; }
        [Index(2)]
        public string LocationCanton { get; set; }
        [Index(3)]
        public string Standort { get; set; }
        [Index(4)]
        public string StreetName { get; set; }
        [Index(5)]
        public string PostalNumberAndMunicipality { get; set; }
    }
}
