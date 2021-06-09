using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.DocumentProperties;
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
        public int Id { get; set; }
        public string RowVersion { get; set; }
        public string Description { get; set; }
        public bool Activity { get; set; }
        public List<string> Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public AddressDTO Address { get; set; }
        public OrganizationDataOut Parent { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public Uri LogoUrl { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<IdentifierDataOut> Identifiers { get; set; }
        public List<DocumentClinicalDomain> ClinicalDomain { get; set; }
        public string Email { get; set; }

    }
}