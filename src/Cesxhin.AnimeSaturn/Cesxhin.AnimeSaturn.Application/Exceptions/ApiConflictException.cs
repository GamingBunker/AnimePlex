using System;

namespace Cesxhin.AnimeSaturn.Application.Exceptions
{
    public class ApiConflictException : Exception
    {
        public ApiConflictException() : base() { }
        public ApiConflictException(string message) : base(message) { }
        public ApiConflictException(string message, Exception inner) : base(message, inner) { }
    }
}
