#region

using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Share.DbContracts;

#endregion

namespace API;

public static class WebApplicationBuilderExtensions {
    public static void AddCurrencyServices(this IServiceCollection services
        , IConfiguration configuration
        , bool useInMemoryDb = false)
    {
        services.AddDbContext<CurrencyDb>(options =>
        {
            options.UseSqlServer("Server=82.115.26.134;Initial Catalog=Currency;User ID=sa;" +
                                 "Password=L(63luggHIkedq5>;TrustServerCertificate=True", sqlOption => { sqlOption.UseNetTopologySuite(); });
        });

        services.AddUoWAndRepoServices<CurrencyDb>();
        services.AddMemoryCache();
    }
}