using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Filter
{
    public class OrganizationFilter
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public string IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string State { get; set; }
        public int? CountryId { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string ClinicalDomain { get; set; }
        public int? Parent { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
    }
}
