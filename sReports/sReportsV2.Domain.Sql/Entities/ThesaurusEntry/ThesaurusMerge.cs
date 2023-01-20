using Newtonsoft.Json;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusMerge : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ThesaurusMergeId")]
        public int ThesaurusMergeId { get; set; }
        public int NewThesaurus { get; set; }
        public int OldThesaurus { get; set; }
        public ThesaurusMergeState State { get; set; }
        
        [NotMapped]
        public List<string> CompletedCollections { get; set; }
        public string CompletedCollectionsString
        {
            get
            {
                return this.CompletedCollections == null || !this.CompletedCollections.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.CompletedCollections);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.CompletedCollections = new List<string>();
                else
                    this.CompletedCollections = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        [NotMapped]
        public List<string> FailedCollections { get; set; }
        public string FailedCollectionsString
        {
            get
            {
                return this.FailedCollections == null || !this.FailedCollections.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.FailedCollections);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.FailedCollections = new List<string>();
                else
                    this.FailedCollections = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }
    }
}
