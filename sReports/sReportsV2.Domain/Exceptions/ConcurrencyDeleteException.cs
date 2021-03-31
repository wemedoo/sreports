using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Exceptions
{
    [Serializable()]
    public class MongoDbConcurrencyDeleteException : Exception
    {
        public MongoDbConcurrencyDeleteException()
        {
        }
        public MongoDbConcurrencyDeleteException(string message)
          : base(message)
        {
        }

        public MongoDbConcurrencyDeleteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MongoDbConcurrencyDeleteException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
