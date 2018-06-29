using System;

namespace services.abstractions.Exceptions
{
    public class LocationIsNotAvailableException : Exception
    {
        public override string Message => "Location is not availble";
    }
}