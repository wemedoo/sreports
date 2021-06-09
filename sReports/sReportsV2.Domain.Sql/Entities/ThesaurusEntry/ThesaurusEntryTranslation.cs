using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusEntryTranslation
    {
        public int Id { get; set; }
        public ThesaurusEntry ThesaurusEntry { get; set; }
        public int ThesaurusEntryId { get; set; }
        public string Language { get; set; }
        public string Definition { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(500)]
        public string PreferredTerm { get; set; }

        [NotMapped]
        public List<string> Synonyms { get; set; }
        public string SynonymsString
        {
            get
            {
                return this.Synonyms == null || !this.Synonyms.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.Synonyms);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Synonyms = new List<string>();
                else
                    this.Synonyms = JsonConvert.DeserializeObject<List<string>>(value);
            }

        }


        [NotMapped]
        public List<string> Abbreviations { get; set; }

        public string AbbreviationsString
        {
            get
            {
                return this.Abbreviations == null || !this.Abbreviations.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.Abbreviations);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Abbreviations = new List<string>();
                else
                    this.Abbreviations = JsonConvert.DeserializeObject<List<string>>(value);
            }


        }
    }
}
