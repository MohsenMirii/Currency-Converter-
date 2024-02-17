

using Share.Helpers;



namespace Share.Exceptions;

public class SecurityError500Exception : Exception {
    public SecurityError500Exception(string message) : base(message)
    {
    }

    public SecurityError500Exception(string logMessage, object details) : base(logMessage
                                                                               + $" [DETAILS: {details.ToJsonStringForDeveloper()}]")
    {
    }
}