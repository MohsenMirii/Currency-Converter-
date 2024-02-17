namespace Share.Contracts;

public interface IHasSoftDelete {
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDateTime { get; set; }
}