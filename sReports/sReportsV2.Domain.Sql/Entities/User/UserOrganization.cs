using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class UserOrganization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public InstitutionalLegalForm? LegalForm { get; set; }
        public InstitutionalOrganizationalForm? OrganizationalForm { get; set; }
        public string DepartmentName { get; set; }
        public string Team { get; set; }
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public UserState? State { get; set; }
        public List<Role> Roles { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
