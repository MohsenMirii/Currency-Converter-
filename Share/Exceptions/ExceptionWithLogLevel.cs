#region

using Microsoft.Extensions.Logging;

#endregion

namespace Share.Exceptions;

public abstract class ExceptionWithLogLevel : Exception {
    protected ExceptionWithLogLevel()
    {
    }

    protected ExceptionWithLogLevel(string[] errors) : base(string.Join("|", errors))
    {
        Errors = errors;
    }

    public string[]? Errors { get; set; }

    public LogLevel LogLevel { get; set; } = LogLevel.None;
}