using System;

namespace Weather.Network
{
    public class ApiKeyException : Exception
    {
        public ApiKeyException()
        {
        }

        public ApiKeyException(string message)
            : base(message)
        {
        }

        public ApiKeyException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
