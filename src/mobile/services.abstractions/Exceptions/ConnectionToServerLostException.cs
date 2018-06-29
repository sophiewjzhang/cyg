namespace services.abstractions.Exceptions
{
    public class ConnectionToServerLostException : GenericNoConnectionException
    {
        public override string Message => "Connection to server has been lost";
    }
}