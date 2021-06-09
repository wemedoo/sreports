using Newtonsoft.Json;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Consensus
{
    public class ConsensusIteration
    {
        public int Id { get; set; }
        public List<ConsensusQuestion> Questions { get; set; }
        public IterationState? State { get; set; }

        [NotMapped]
        public List<int> OutsideUserIds { get; set; }
        public string OutsideUserIdsString
        {
            get
            {
                return this.OutsideUserIds == null || !this.OutsideUserIds.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.OutsideUserIds);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.OutsideUserIds = new List<int>();
                else
                    this.OutsideUserIds = JsonConvert.DeserializeObject<List<int>>(value);
            }

        }

        [NotMapped]
        public List<int> UserIds { get; set; }
        public string UserIdsString
        {
            get
            {
                return this.UserIds == null || !this.UserIds.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.UserIds);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.UserIds = new List<int>();
                else
                    this.UserIds = JsonConvert.DeserializeObject<List<int>>(value);
            }

        }
    }
}
