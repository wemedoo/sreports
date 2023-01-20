using System;

namespace sReportsV2.Common.Exceptions
{
    [Serializable]
    public class ApiCallException : Exception
    {
        public ApiCallException(string message) : base(message)
        {
        }
    }
}
