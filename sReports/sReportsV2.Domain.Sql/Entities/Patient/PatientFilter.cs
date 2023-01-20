using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class PatientFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public string City { get; set; }
        public int? CountryId { get; set; }
        public string PostalCode { get; set; }
        public int OrganizationId { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
        public List<string> Genders { get; set; } = new List<string>();
        public List<string> Activity { get; set; } = new List<string>();
        public string GetPatientId()
        {
            string result = null;
            if (!string.IsNullOrEmpty(IdentifierType) && IdentifierType.Equals(ResourceTypes.O4PatientId) && !string.IsNullOrEmpty(IdentifierValue))
            {
                result = IdentifierValue;
            }
            return result;
        }
    }
}
