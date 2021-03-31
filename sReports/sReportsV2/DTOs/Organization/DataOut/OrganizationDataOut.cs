using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization
{
    public class OrganizationDataOut
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public bool Activity { get; set; }
        public List<string> Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<TelecomDTO> Telecom { get; set; }
        public AddressDTO Address { get; set; }
        public OrganizationDataOut PartOf { get; set; }
        public List<OrganizationDataOut> Ancestors { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public Uri LogoUrl { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<IdentifierDataOut> Identifiers { get; set; }
    }
}