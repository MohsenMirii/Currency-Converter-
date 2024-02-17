

using Share.Helpers;



namespace Share.Exceptions;

// todo: migrate all exceptions to 'ServerError500Exception'
public class ServerError500Exception : Exception {
    public ServerError500Exception(string message) : base(message)
    {
    }

    public ServerError500Exception(string logMessage, object details) : base(logMessage
                                                                             + $" [DETAILS: {details.ToJsonStringForDeveloper()}]")
    {
    }
}