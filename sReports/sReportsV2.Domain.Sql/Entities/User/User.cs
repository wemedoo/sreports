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
    public class User : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public UserPrefix Prefix { get; set; }
        public virtual List<UserAcademicPosition> AcademicPositions { get; set; } = new List<UserAcademicPosition>();
        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        public int UserConfigId { get; set; }
        [ForeignKey("UserConfigId")]
        public virtual UserConfig UserConfig { get; set; }
        public virtual List<UserOrganization> Organizations { get; set; } = new List<UserOrganization>();
        public virtual List<UserClinicalTrial> ClinicalTrials { get; set; } = new List<UserClinicalTrial>();
        public virtual List<UserRole> Roles { get; set; } = new List<UserRole>();

        public List<UserOrganization> GetNonArchivedOrganizations()
        {
            return Organizations.Where(x => x.State != UserState.Archived).ToList();
        }

        public IEnumerable<UserOrganization> GetActiveOrganizations()
        {
            return Organizations.Where(x => x.State == UserState.Active);
        }

        public List<int> GetOrganizationRefs()
        {
            if (this.Organizations == null)
                this.Organizations = new List<UserOrganization>();

            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }

        public void AddSuggestedForm(string formId)
        {
            this.UserConfig.AddSuggestedForm(formId);
        }

        public void RemoveSuggestedForm(string formId)
        {
            this.UserConfig.RemoveSuggestedForm(formId);
        }

        public void AddClinicalTrial(UserClinicalTrial clinicalTrial)
        {
            if (ClinicalTrials == null)
            {
                ClinicalTrials = new List<UserClinicalTrial>();
            }

            ClinicalTrials.Add(clinicalTrial);
        }

        public void Copy(User user)
        {
            this.Username = user.Username;
            this.Prefix = user.Prefix;
            this.FirstName = user.FirstName;
            this.MiddleName = user.MiddleName;
            this.LastName = user.LastName;
            this.DayOfBirth = user.DayOfBirth;
            this.Email = user.Email;
            this.PersonalEmail = user.PersonalEmail;
            if (Address == null)
            {
                Address = new Address();
            }
            this.Address.Copy(user.Address);
            
        }

        public void SetUserConfig(string activeLanguage)
        {
            if (this.UserConfig == null)
            {
                this.UserConfig = new UserConfig();
            }
            this.UserConfig.ActiveLanguage = activeLanguage;
        }

        public void ConfigureAcademicPositions(List<AcademicPosition> academicPositions)
        {
            AddAcademicPositions(academicPositions);
            RemoveAcademicPositions(academicPositions);
        }

        public void UpdateRoles(List<int> newSelectedRoles)
        {
            if (RolesHaveChanged(newSelectedRoles))
            {
                AddUserRoles(newSelectedRoles);
                RemoveUserRoles(newSelectedRoles);
            }   
        }

        public bool UserHasPermission(string permissionName, string moduleName)
        {
            return Roles
                .Where(r => !r.IsDeleted)
                .SelectMany(r => r.Role.Permissions)
                .Any(p => p.Module.Name.Equals(moduleName) && p.Permission.Name.Equals(permissionName));
        }

        private void AddAcademicPositions(List<AcademicPosition> academicPositions)
        {
            if (academicPositions != null)
            {
                foreach (var academicPosition in academicPositions)
                {
                    UserAcademicPosition userAcademicPosition = AcademicPositions.FirstOrDefault(x => x.AcademicPositionTypeId == (int)academicPosition && !x.IsDeleted);
                    if (userAcademicPosition == null)
                    {
                        AcademicPositions.Add(new UserAcademicPosition()
                        {
                            UserId = UserId,
                            AcademicPositionTypeId = (int)academicPosition
                        });
                    }
                }
            }
        }

        private void RemoveAcademicPositions(List<AcademicPosition> academicPositions)
        {
            foreach (UserAcademicPosition userAcademic in AcademicPositions)
            {
                AcademicPosition? academicPosition = academicPositions.FirstOrDefault(x => (int)x == userAcademic.AcademicPositionTypeId);
                if (academicPosition == 0)
                {
                    userAcademic.IsDeleted = true;
                    userAcademic.SetLastUpdate();
                }
            }
        }

        private bool RolesHaveChanged(List<int> newRoles)
        {
            return !newRoles.SequenceEqual(GetRolesIds());
        }

        private List<int> GetRolesIds()
        {
            return Roles.Where(x => !x.IsDeleted).Select(x => x.RoleId).ToList();
        }

        private void AddUserRoles(List<int> newRoles)
        {
            if (newRoles != null)
            {
                foreach (var userRoleId in newRoles)
                {
                    UserRole userRole = Roles.FirstOrDefault(x => x.RoleId == userRoleId && !x.IsDeleted);
                    if (userRole == null)
                    {
                        Roles.Add(new UserRole()
                        {
                            UserId = UserId,
                            RoleId = userRoleId
                        });
                    }
                }
            }
        }

        private void RemoveUserRoles(List<int> newRoles)
        {
            foreach (UserRole userRole in Roles)
            {
                int roleId = newRoles.FirstOrDefault(x => x == userRole.RoleId);
                if (roleId == 0)
                {
                    userRole.IsDeleted = true;
                    userRole.SetLastUpdate();
                }
            }
        }
    }
}
