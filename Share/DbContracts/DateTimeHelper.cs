namespace Share.DbContracts;

public static class DbDateTime {
    public static DateTimeOffset Now => DateTimeOffset.UtcNow;

    public static DateTimeOffset Today => DateTimeOffset.Now.Date.ToUniversalTime();
}