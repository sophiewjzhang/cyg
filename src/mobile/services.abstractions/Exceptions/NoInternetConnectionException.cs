namespace services.abstractions.Exceptions
{
    public class NoInternetConnectionException : GenericNoConnectionException
    {
        public override string Message => "No internet connection";
    }
}