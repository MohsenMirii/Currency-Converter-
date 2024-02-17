using System.Reflection;
using Data.DbContexts;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scrutor;
using Scrutor.AspNetCore;
using Share.DbContracts;
using Share.Misc;

namespace API;

public static class GeneralHelpers {
    private static readonly Lazy<Assembly[]> AllAssemblies = new(() =>
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
        var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

        var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

        return loadedAssemblies.Where(r => r.FullName!.StartsWith("API")
                                           || r.FullName!.StartsWith("Data")
                                           || r.FullName!.StartsWith("Core")
                                           || r.FullName!.StartsWith("Share")).ToArray();
    });

    [PublicAPI]
    public static IServiceCollection AutoRegisterMediatR(this IServiceCollection services)
    {
        services.AddMediatR(AllAssemblies.Value);

        return services;
    }

    [PublicAPI]
    public static IServiceCollection AutoRegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICancellation, Cancellation>();

        serviceCollection.AddDbContext<CurrencyDb>(options =>
        {
            options.UseSqlServer("Server=82.115.26.134;Initial Catalog=Currency;User ID=sa;" +
                                 "Password=L(63luggHIkedq5>;TrustServerCertificate=True",
                sqlOption => { sqlOption.UseNetTopologySuite(); });
        });

        serviceCollection.AddUoWAndRepoServices<CurrencyDb>();
        serviceCollection.AddMemoryCache();
        return serviceCollection.Scan(setup => setup
            .FromAssemblies(AllAssemblies.Value)
            .AddClasses(classes => classes.AssignableTo<ISingletonLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<ISelfSingletonLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<ISelfTransientLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISelfScopedLifetime>(), false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithScopedLifetime());
        
        
    }
}