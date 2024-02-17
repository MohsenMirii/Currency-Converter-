

using Share.Helpers;



namespace Share.Exceptions;

public class Impossible500Exception : ExceptionWithLogLevel {
    public Impossible500Exception()
    {
    }

    public Impossible500Exception(string logMessage, object details) : base(new[]
    {
        logMessage + $" [DETAILS: {details.ToJsonStringForDeveloper()}]"
    })
    {
    }

    public Impossible500Exception(object details) : base(new[] { details.ToJsonStringForDeveloper() })
    {
    }
}