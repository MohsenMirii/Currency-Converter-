#region

using JetBrains.Annotations;

#endregion

namespace Share.Exceptions;

[PublicAPI]
public class BadRequest400Exception : ExceptionWithLogLevel {
    public BadRequest400Exception(params string[] errors) : base(errors)
    {
    }

    public BadRequest400Exception(ErrorCode code, params string[] message) : base(message)
    {
        Code = code;
    }

    public ErrorCode? Code { get; set; } = StandardErrorCodes.Validation;
}