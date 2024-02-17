#region

using System.Reflection;
using JetBrains.Annotations;
using MediatR;
using Scrutor;
using Scrutor.AspNetCore;
using Share.Misc;

#endregion

namespace API;

public static class GeneralHelpers {
    private const string AssemblyPrefix = "Feed.";

    private static readonly Lazy<Assembly[]> AllAssemblies = new(() =>
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
        var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

        var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

        return loadedAssemblies.Where(r => r.FullName!.StartsWith(AssemblyPrefix)).ToArray();
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