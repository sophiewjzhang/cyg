using System;

namespace services.abstractions.Exceptions
{
    public class GenericNoConnectionException : Exception
    {
        public override string Message => "An error has occured";
    }
}