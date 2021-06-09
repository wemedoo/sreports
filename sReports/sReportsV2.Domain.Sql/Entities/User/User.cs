using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class User : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public UserPrefix Prefix { get; set; }
        public List<AcademicPosition> AcademicPositions { get; set; } = new List<AcademicPosition>();
        public int? AddressId { get; set; }
        public virtual Address Address { get; set; }
        public int UserConfigId { get; set; }
        public virtual UserConfig UserConfig { get; set; }
        public List<string> SuggestedForms { get; set; }
        public int? ActiveOrganizationId { get; set; }  
        public Organization ActiveOrganization { get; set; }
        public virtual List<UserOrganization> Organizations { get; set; } = new List<UserOrganization>();
        public virtual List<UserClinicalTrial> ClinicalTrials { get; set; } = new List<UserClinicalTrial>();
        public virtual List<Role> Roles { get; set; } = new List<Role>();
        
        public List<Role> GetRolesByActiveOrganization(int activeOrganizationId)
        {
            return this.Organizations.FirstOrDefault(x => x.Id == activeOrganizationId)?.Roles;
        }

        public List<int> GetOrganizationRefs()
        {
            if (this.Organizations == null)
                this.Organizations = new List<UserOrganization>();

            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }
        public void AddSuggestedForm(string formId)
        {
            if (this.SuggestedForms == null)
            {
                this.SuggestedForms = new List<string>();
            }

            this.SuggestedForms.Add(formId);
        }

        public void RemoveSuggestedForm(string formId)
        {

            this.SuggestedForms = this.SuggestedForms.Where(x => !string.IsNullOrWhiteSpace(formId)).ToList();
        }

        public void AddClinicalTrial(UserClinicalTrial clinicalTrial)
        {
            if (ClinicalTrials == null)
            {
                ClinicalTrials = new List<UserClinicalTrial>();
            }

            ClinicalTrials.Add(clinicalTrial);
        }
    }
}
