using System.Reflection;
using Data.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.DbContracts;

namespace Data.DbContexts;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CurrencyDb : DbContext {
    private readonly Assembly _assembly = typeof(CurrencyDb).Assembly;
    protected readonly Lazy<Type[]> Entities;

    public CurrencyDb()
    {
    }

    public CurrencyDb(DbContextOptions<CurrencyDb> options) : base(options)
    {
        Entities = new Lazy<Type[]>(() => _assembly
            .GetExportedTypes()
            .Where(r => typeof(IEntity).IsAssignableFrom(r))
            .ToArray()
        );
    }

    public DbSet<ConversionRate> ConversionRates { get; set; } = null!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder
                .UseSqlServer("Server=82.115.26.134;Initial Catalog=Currency;User ID=sa;" +
                              "Password=L(63luggHIkedq5>;TrustServerCertificate=True"
                );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(_assembly);
        _registerGlobalQueryFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Currency");
    }

    protected void _registerGlobalQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var type in Entities.Value)
        {
            if (type == typeof(HasFullAudit))
                continue;

            var hasSoftDelete = typeof(IHasSoftDelete).IsAssignableFrom(type);

            if (hasSoftDelete)
                SetSoftDeleteFilterMethod.MakeGenericMethod(type).Invoke(this, new object[] { modelBuilder });
        }
    }

    private static MethodInfo SetSoftDeleteFilterMethod =>
        typeof(BaseDbContext<CurrencyDb>)
            .GetMethod(nameof(_setSoftDeleteFilterGeneric), BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new Exception("method not found");

    private static void _setSoftDeleteFilterGeneric<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IHasSoftDelete
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.IsDeleted);
    }
}