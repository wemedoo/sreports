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
        [Column("UserOrganizationId")]
        public int UserOrganizationId { get; set; }
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public UserState? State { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public void Copy(UserOrganization userOrganization)
        {
            this.IsPracticioner = userOrganization.IsPracticioner;
            this.Qualification = userOrganization.Qualification;
            this.SeniorityLevel = userOrganization.SeniorityLevel;
            this.Speciality = userOrganization.Speciality;
            this.State = userOrganization.State;
            this.SubSpeciality = userOrganization.SubSpeciality;
        }
    }
}
