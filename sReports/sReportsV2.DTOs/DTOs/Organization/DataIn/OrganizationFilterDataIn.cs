using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataIn
{
    public class OrganizationFilterDataIn : Common.DataIn
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Activity { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public string IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string ClinicalDomain { get; set; }
        public int? Parent { get; set; }
    }
}