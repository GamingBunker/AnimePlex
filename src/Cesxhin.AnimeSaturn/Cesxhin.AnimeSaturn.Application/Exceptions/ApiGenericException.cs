using System;

namespace Cesxhin.AnimeSaturn.Application.Exceptions
{
    public class ApiGenericException : Exception
    {
        public ApiGenericException() : base() { }
        public ApiGenericException(string message) : base(message) { }
        public ApiGenericException(string message, Exception inner) : base(message, inner) { }
    }
}
