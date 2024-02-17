namespace Share.Contracts;

public interface IHasUpdateDateTime {
    public DateTimeOffset? UpdateDateTime { get; set; }
}