

using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;
using Share.Contracts;



namespace Share.DbContracts;

public static class Extensions {
    [PublicAPI]
    public static IServiceCollection AddUoWAndRepoServices<TDb>(this IServiceCollection services)
        where TDb : DbContext
    {
        var entities = typeof(TDb).Assembly
            .GetTypes()
            .Where(r => typeof(IEntity).IsAssignableFrom(r) && !r.IsAbstract)
            .ToArray();

        foreach (var entity in entities)
        {
            var isNotReadOnly = typeof(IHasSoftDelete).IsAssignableFrom(entity);

            if (isNotReadOnly)
                services.AddScoped(typeof(IRepo<>).MakeGenericType(entity),
                    typeof(GenericRepo<,>).MakeGenericType(entity, typeof(TDb)));

            services.AddScoped(typeof(IReadOnlyRepo<>).MakeGenericType(entity),
                typeof(ReadOnlyRepo<,>).MakeGenericType(entity, typeof(TDb)));
        }

        return services;
    }

    public static void RegisterEdmModel(Assembly assembly)
    {
        var entities = assembly
            .GetTypes()
            .Where(r => typeof(IOData).IsAssignableFrom(r))
            .ToArray();

        foreach (var entity in entities)
            AddEntitySet(ODataIQueryableExtension.Builder, entity);
    }

    [PublicAPI]
    public static IServiceCollection AddDbContext<TDb>(this IServiceCollection services
        , IConfiguration configuration
        , string connectionStringName
        , bool useInMemory = false)
        where TDb : DbContext
    {
        return services.AddDbContext<TDb>(setup =>
        {
            if (useInMemory)
            {
                setup.UseInMemoryDatabase(Guid.NewGuid().ToString());

                return;
            }

            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = configuration.GetConnectionString("Default");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("'ConnectionStrings:Identity' is not defined");

            // todo: remove this line ASAP
            Console.WriteLine("connectionString:" + connectionString);

            setup.UseSqlServer(connectionString);
            // setup.UseSnakeCaseNamingConvention();
            setup.EnableSensitiveDataLogging();
            // setup.UseBatchEF_Npgsql();
        });
    }

    public static UpdateBuilder<TEntity> UpdateWhere<TEntity>(this IRepo<TEntity> repo
        , Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IHasSoftDelete
    {
        return repo.Update().Where(predicate);
    }

    private static void AddEntitySet(ODataConventionModelBuilder builder, Type type)
    {
        // todo: fix this lines 
        var name = type.Name[1..].ToLower() + "s";
        var entityType = builder.AddEntityType(type);
        builder.AddEntitySet(name, entityType);
    }
}