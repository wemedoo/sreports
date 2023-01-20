using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.User.DataIn
{
    public class UserDataIn
    {
        public int Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }

        public string ContactPhone { get; set; }
        public List<int> Roles { get; set; } = new List<int>();
        public UserPrefix Prefix { get; set; }
        public List<AcademicPosition> AcademicPositions { get; set; } = new List<AcademicPosition>();
        public string MiddleName { get; set; }
        public AddressDataIn Address { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public List<UserOrganizationDataIn> UserOrganizations { get; set; }
        public List<ClinicalTrialDTO> ClinicalTrials { get; set; }
       

    }
}