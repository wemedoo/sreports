using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.DTOs.DTOs.User.DataOut
{
    public class UserViewDataOut
    {
        public int Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserState? State { get; set; }
        public string Roles { get; set; }
        public string UserOrganizations { get; set; }
        public bool IsUserBlocked(int activeOrganizationId)
        {
            return State == UserState.Blocked ? true : false;
        }
    }
}
