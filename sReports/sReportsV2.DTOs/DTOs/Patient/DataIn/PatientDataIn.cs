using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization.DataIn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient
{
    public class PatientDataIn
    {
        public string Id { get; set; }
        public bool Activity { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public ContactDTO ContactPerson{ get; set; }
        public List<IdentifierDataIn> Identifiers { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public AddressDTO Address { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}