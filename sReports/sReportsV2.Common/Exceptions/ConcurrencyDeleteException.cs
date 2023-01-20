using System;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class ConcurrencyDeleteException : Exception
    {
        public ConcurrencyDeleteException()
        {
        }
        public ConcurrencyDeleteException(string message)
          : base(message)
        {
        }

        public ConcurrencyDeleteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ConcurrencyDeleteException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
