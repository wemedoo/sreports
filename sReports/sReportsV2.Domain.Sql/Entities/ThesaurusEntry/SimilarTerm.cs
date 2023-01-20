﻿using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class SimilarTerm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("SimilarTermId")]
        public int SimilarTermId { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(2950)]
        public string Name { get; set; }
        public string Definition { get; set; }
        public SimilarTermType Source { get; set; }
        public DateTime? EntryDateTime { get; set; }
        [ForeignKey("ThesaurusEntryTranslationId")]
        public ThesaurusEntryTranslation ThesaurusEntryTranslation { get; set; }
        public int ThesaurusEntryTranslationId { get; set; }

    }
}
