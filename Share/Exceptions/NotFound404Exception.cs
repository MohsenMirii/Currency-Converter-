

using Microsoft.Extensions.Logging;



namespace Share.Exceptions;

public class NotFound404Exception : ExceptionWithLogLevel {
    public NotFound404Exception(string source, params object[] keys) : base(new[]
    {
        source + ": " + string.Join(", ", keys.Select(r => $"[{r}]"))
    })
    {
    }

    public NotFound404Exception(Type source, params object[] keys) : this(source.ToString(), keys)
    {
    }

    public NotFound404Exception(LogLevel logLevel, Type source, params object[] keys) : this(source.ToString(), keys)
    {
        LogLevel = logLevel;
    }
}