namespace Share.Contracts;

public interface IHasCreateDateTime {
    DateTimeOffset CreateDateTime { get; set; }
}