using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Identifier
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string System { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }

        public Identifier() { }

        public Identifier(string system, string value)
        {
            this.System = system;
            this.Value = value;
        }

        public Identifier(string system, string value, string type, string use)
        {
            this.System = system;
            this.Value = value;
            this.Type = type;
            this.Use = use;
        }
    }
}
