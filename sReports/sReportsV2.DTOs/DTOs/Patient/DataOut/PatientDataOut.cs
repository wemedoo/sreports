using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public int? CitizenshipId { get; set; }
        public int? ReligionId { get; set; }
        public DateTime? DeceasedDateTime { get; set; }
        public bool? Deceased { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TelecomDTO> ContactTelecoms { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public List<AddressDTO> Addresses { get; set; }
        public AddressDTO ContactAddress { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string CityNames
        {
            get
            {
                return string.Join(", ", Addresses.Select(a => a.City).Where(c => !string.IsNullOrEmpty(c)));
            }
        }

        public List<EpisodeOfCareDataOut> EpisodeOfCares { get; set; }

        public EpisodeOfCareDataOut GetEpisodeOfCare(int id)
        {
            return EpisodeOfCares.FirstOrDefault(eoc => eoc.Id == id);
        }

        public void ReplaceEpisodeOfCare(int id, EpisodeOfCareDataOut eocReplacement)
        {
            EpisodeOfCareDataOut eocToReplace = GetEpisodeOfCare(id);
            int eocIndex = EpisodeOfCares.IndexOf(eocToReplace);
            if (eocIndex != -1)
            {
                EpisodeOfCares[eocIndex] = eocReplacement;
            }
        }
    }
}