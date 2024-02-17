namespace Share.Exceptions;

public class Unauthorized401Exception : ExceptionWithLogLevel {
    public Unauthorized401Exception()
    {
    }

    public Unauthorized401Exception(string message) : base(new[] { message })
    {
    }
}