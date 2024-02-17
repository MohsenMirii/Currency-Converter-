#region

using Data.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Data.DbContexts;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CurrencyDb : DbContext {
    public CurrencyDb(DbContextOptions<CurrencyDb> options) : base(options)
    {
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
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Currency");
    }
}