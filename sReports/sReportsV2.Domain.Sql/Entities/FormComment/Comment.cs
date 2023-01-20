using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.FormComment
{
    public class Comment : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CommentId")]
        public int CommentId { get; set; }
        public CommentState CommentState { get; set; }
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public int? CommentRef { get; set; }
        public string FormRef { get; set; }
        public int UserId { get; set; }
    }
}
