using System;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class ConcurrencyDeleteEditException : Exception
    {
        public ConcurrencyDeleteEditException()
        {
        }

        public ConcurrencyDeleteEditException(string message)
            : base(message)
        {
        }

        public ConcurrencyDeleteEditException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ConcurrencyDeleteEditException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
