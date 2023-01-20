using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.Organization.DataIn;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Patient
{
    public class PatientDataIn
    {
        public string Id { get; set; }
        public bool Activity { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        [Year]
        public DateTime? BirthDate { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public ContactDTO ContactPerson{ get; set; }
        public List<IdentifierDataIn> Identifiers { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public List<AddressDataIn> Addresses { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int? CitizenshipId { get; set; }
        public int? ReligionId { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }
    }
}