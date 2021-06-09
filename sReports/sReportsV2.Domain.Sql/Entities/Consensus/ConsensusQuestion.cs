using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Consensus
{
    public class ConsensusQuestion
    {
        public int Id { get; set; }
        public string ItemRef { get; set; }
        public string Question { get; set; }
        public string Comment { get; set; }
        public string Value { get; set; }
        public int ConsensusIterationId { get; set; }
        [NotMapped]
        public List<string> Options { get; set; }
        public string OptionsString
        {
            get
            {
                return this.Options == null || !this.Options.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.Options);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Options = new List<string>();
                else
                    this.Options = JsonConvert.DeserializeObject<List<string>>(value);
            }

        }
    }
}
