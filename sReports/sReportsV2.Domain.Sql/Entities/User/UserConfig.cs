using Newtonsoft.Json;
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
    public class UserConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("UserConfigId")]
        public int UserConfigId { get; set; }
        public int PageSize { get; set; } = 5;
        public string ActiveLanguage { get; set; }
        public string TimeZoneOffset { get; set; }
        public int? ActiveOrganizationId { get; set; }
        [ForeignKey("ActiveOrganizationId")]
        public virtual Organization ActiveOrganization { get; set; }
        [NotMapped]
        public List<string> SuggestedForms { get; set; }

        public string SuggestedFormsString
        {
            get
            {
                return this.SuggestedForms == null || !this.SuggestedForms.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.SuggestedForms);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.SuggestedForms = new List<string>();
                else
                    this.SuggestedForms = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        public void AddSuggestedForm(string formId)
        {
            this.SuggestedForms.Add(formId);
        }

        public void RemoveSuggestedForm(string formId)
        {
            this.SuggestedForms = this.SuggestedForms.Where(x => x != formId).ToList();
        }

        [NotMapped]
        public List<string> PredefinedForms { get; set; }

        public string PredefinedFormsString
        {
            get
            {
                return this.PredefinedForms == null || !this.PredefinedForms.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.PredefinedForms);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.PredefinedForms = new List<string>();
                else
                    this.PredefinedForms = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        public void AddPredefinedForm(string formId)
        {
            this.PredefinedForms.Add(formId);
        }

        public List<string> GetForms()
        {
            return this.SuggestedForms != null && this.SuggestedForms.Count > 0 ? this.SuggestedForms : this.PredefinedForms;
        }
    }
}
