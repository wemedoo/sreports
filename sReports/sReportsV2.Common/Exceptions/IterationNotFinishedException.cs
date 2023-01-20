using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Exceptions
{
    [Serializable]
    public class IterationNotFinishedException : Exception
    {
        public IterationNotFinishedException()
        {
        }
        public IterationNotFinishedException(string message)
          : base(message)
        {
        }

        public IterationNotFinishedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected IterationNotFinishedException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
