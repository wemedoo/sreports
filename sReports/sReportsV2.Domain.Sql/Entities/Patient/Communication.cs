using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Communication
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public bool Preferred { get; set; } 
        public Communication() { }
        public Communication(string language, bool preferred)
        {
            this.Language = language;
            this.Preferred = preferred;
        }
    }
}
