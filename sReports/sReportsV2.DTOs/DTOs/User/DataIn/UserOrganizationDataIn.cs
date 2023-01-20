using sReportsV2.Common.Enums;

namespace sReportsV2.DTOs.User.DataIn
{
    public class UserOrganizationDataIn
    {
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public int OrganizationId { get; set; }
        public UserState? State { get; set; }
    }
}