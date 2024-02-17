namespace Share.Exceptions;

public class ErrorCode {
    public ErrorCode(string code)
    {
        Code = code;
    }

    public string Code { get; init; }
}

public static class StandardErrorCodes {
    public static readonly ErrorCode Unknown = new("Unknown");
    public static readonly ErrorCode Validation = new("VALIDATION");
    public static readonly ErrorCode AutoValidation = new("AUTO_VALIDATION");
    public static readonly ErrorCode NotFound = new("NOT_FOUND");
    public static readonly ErrorCode Timeout = new("TIMEOUT");
    public static readonly ErrorCode Unauthorized = new("UNAUTHORIZED");
    public static readonly ErrorCode Forbidden = new("FORBIDDEN");
    public static readonly ErrorCode Server = new("SERVER");
}