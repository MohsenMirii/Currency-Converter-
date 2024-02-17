namespace Share.Contracts;

public abstract class HasFullAudit : IEntity, IHasLongId, IHasCreateDateTime, IHasCreatorUserId, IHasUpdateDateTime,
    IHasSoftDelete {
    public DateTimeOffset CreateDateTime { get; set; }

    public long CreatorUserId { get; set; }

    public long Id { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDateTime { get; set; }

    public DateTimeOffset? UpdateDateTime { get; set; }
}