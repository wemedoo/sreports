using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Consensus
{
    public class ConsensusInstance : EntitiesBase.Entity
    {
        public int Id { get; set; }
        public int ConsensusRef { get; set; }
        public int UserId { get; set; }
        public bool IsOutsideUser { get; set; }
        public List<ConsensusQuestion> Questions { get; set; }
        public int IterationId { get; set; }

        public double GetPercentDone()
        {
            return this != null ?
                this.Questions.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Count() / (double)Questions.Count * 100
                : 0;
        }
    }
}
