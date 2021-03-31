using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Exceptions
{
    [Serializable()]
    public class MongoDbConcurrencyException : Exception
    {
        public MongoDbConcurrencyException()
        {
        }

        public MongoDbConcurrencyException(string message)
            : base(message)
        {
        }

        public MongoDbConcurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MongoDbConcurrencyException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}