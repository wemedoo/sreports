using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient.DataIn
{
    public class PatientFilterDataIn : Common.DataIn
    {
        public DateTime? BirthDate { get; set; }
        public string IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string Family { get; set; }
        public string Given { get; set; }
        public string City { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public int OrganizationId { get; set; }
        public List<string> Genders { get; set; } = new List<string>();
        public List<string> Activity { get; set; } = new List<string>();

    }
}