using System;

namespace Cesxhin.AnimeSaturn.Application.Exceptions
{
    public class ApiNotFoundException : Exception
    {
        public ApiNotFoundException() : base() { }
        public ApiNotFoundException(string message) : base(message) { }
        public ApiNotFoundException(string message, Exception inner) : base(message, inner) { }

    }
}
