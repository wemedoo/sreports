using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataIn;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Organization
{
    public class OrganizationDataIn
    {
        public int? Id { get; set; }
        public string RowVersion { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Impressum { get; set; }
        public List<string> Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<TelecomDTO> Telecom { get; set; }
        public AddressDataIn Address { get; set; }
        public int? AddressId { get; set; }
        public int? ParentId { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public Uri LogoUrl { get; set; }
        public List<IdentifierDataIn> Identifiers { get; set; }
        public List<DocumentClinicalDomain> ClinicalDomain { get; set; }

    }
}