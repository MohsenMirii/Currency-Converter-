

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;



namespace Share.DbContracts;

public abstract class BaseDbContext<TDb>
    : DbContext
    where TDb : DbContext {
    private readonly Assembly _assembly = typeof(TDb).Assembly;
    protected readonly Lazy<Type[]> Entities;

    protected BaseDbContext(DbContextOptions<TDb> options) : base(options)
    {
        Entities = new Lazy<Type[]>(() => _assembly
            .GetExportedTypes()
            .Where(r => typeof(IEntity).IsAssignableFrom(r))
            .ToArray()
        );
    }

    private static MethodInfo SetSoftDeleteFilterMethod =>
        typeof(BaseDbContext<TDb>)
            .GetMethod(nameof(_setSoftDeleteFilterGeneric), BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new Exception("method not found");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(_assembly);
        _registerGlobalQueryFilter(modelBuilder);
    }

    protected void _registerGlobalQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var type in Entities.Value)
        {
            var hasSoftDelete = typeof(IHasSoftDelete).IsAssignableFrom(type);

            if (hasSoftDelete)
                SetSoftDeleteFilterMethod.MakeGenericMethod(type).Invoke(this, new object[] { modelBuilder });
        }
    }

    private static void _setSoftDeleteFilterGeneric<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IHasSoftDelete
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.IsDeleted);
    }
}