using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Consensus
{
    public class QuestionOccurenceConfig
    {
        public string Level { get; set; }
        public QuestionOccurenceType Type { get; set; }
    }
}
