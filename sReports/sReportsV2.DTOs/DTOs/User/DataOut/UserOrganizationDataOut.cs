using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Organization;

namespace sReportsV2.DTOs.User.DataOut
{
    public class UserOrganizationDataOut
    {
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
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