using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Exceptions
{
    [Serializable]
    public class ConsensusCannotStartException : Exception
    {
        public FormItemLevel FormItemLevel { get; set; }
        public ConsensusCannotStartException()
        {
        }
        public ConsensusCannotStartException(FormItemLevel formItemLevel) : base($"Cannot start consensus finding process because all questions are not defined for: {formItemLevel}")
        {
            this.FormItemLevel = formItemLevel;
        }
    }
}
