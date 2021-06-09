using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataOut;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient
{
    public class PatientDataOut
    {
        public List<IdentifierDataOut> Identifiers { get; set; }
        public int Id { get; set; }
        public bool Activity { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactFamily { get; set; }
        public string ContactGender { get; set; }
        public string Relationship { get; set; }
        public string Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TelecomDTO> ContactTelecoms { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public AddressDTO Address { get; set; }
        public AddressDTO ContactAddress { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTime? LastUpdate { get; set; }

        public List<EpisodeOfCareDataOut> EpisodeOfCares { get; set; }
    }
}