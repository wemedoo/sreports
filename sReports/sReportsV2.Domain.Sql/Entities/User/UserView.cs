using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.User
{
    [Table("UserViews")]
    public class UserView : Entity
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public UserPrefix Prefix { get; set; }
        public int? AddressId { get; set; }
        public int UserConfigId { get; set; }
        public string Salt { get; set; }
        public int? OrganizationId { get; set; }
        public UserState? State { get; set; }
        public string Roles { get; set; }
        public string UserOrganizations { get; set; }
    }
}
