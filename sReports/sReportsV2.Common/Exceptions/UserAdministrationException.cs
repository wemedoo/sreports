using System;
using System.Net;

namespace sReportsV2.Common.Exceptions
{
    [Serializable()]
    public class UserAdministrationException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public UserAdministrationException()
        {
        }

        public UserAdministrationException(string message) : base(message)
        {
        }

        public UserAdministrationException(HttpStatusCode httpStatusCode, string message) : this(message)
        {
            this.HttpStatusCode = httpStatusCode;
        }
    }
}
