using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Consensus
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class Consensus : EntitiesBase.Entity
    {
        public int Id { get; set; }
        public string FormRef { get; set; }
        public ConsensusFindingState? State { get; set; }
        public List<ConsensusIteration> Iterations { get; set; }

        public List<int> GetAllUserIds(bool isOutsideUsers)
        {
            List<int> result = new List<int>();
            foreach (ConsensusIteration iteration in this.Iterations)
            {
                if (!isOutsideUsers)
                {
                    result.AddRange(iteration.UserIds);
                }
                else
                {
                    result.AddRange(iteration.OutsideUserIds);
                }
            }

            return result.Distinct().ToList();
        }
    }
}
