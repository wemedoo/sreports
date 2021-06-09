using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.User.DTO;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DataOut
{
    public class UserOrganizationDataOut
    {
        public InstitutionalLegalForm? LegalForm { get; set; }
        public InstitutionalOrganizationalForm? OrganizationalForm { get; set; }
        public string DepartmentName { get; set; }
        public string Team { get; set; }
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public int OrganizationId { get; set; }
        public UserState? State { get; set; }

        public OrganizationDataOut Organization { get; set; }

        public UserOrganizationDataOut() { }

        public UserOrganizationDataOut(OrganizationDataOut organization)
        {
            this.Organization = organization;
            this.OrganizationId = organization.Id;
        }
    }
}