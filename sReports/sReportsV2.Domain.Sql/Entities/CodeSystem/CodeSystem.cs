using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.CodeSystem
{
    public class CodeSystem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CodeSystemId")]
        public int CodeSystemId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string SAB { get; set; }

        public void Copy(CodeSystem codeSystem)
        {
            Value = codeSystem.Value;
            Label = codeSystem.Label;
            SAB = codeSystem.SAB;
        }
    }
}
