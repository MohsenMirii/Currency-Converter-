#region

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

#endregion

namespace Share.DbContracts;

public static class ODataIQueryableExtension {
    public const int DefaultTop = 10;
    public const int MaxTop = 100;
    public static readonly ODataConventionModelBuilder Builder = new();
    public static Lazy<IEdmModel?> DefaultEdmModel = new(() => Builder.GetEdmModel());
    private static readonly Type[] NumberTypes = { typeof(int), typeof(long), typeof(short), typeof(byte) };
    private static readonly Type[] DecimalTypes = { typeof(decimal), typeof(float), typeof(double) };
    private static readonly Type[] DateTimeTypes = { typeof(DateTime), typeof(DateTimeOffset) };

    public static async Task<ODataQueryOut<T>> AsOData<T>(this IQueryable query
        , ODataQueryIn input
        , CancellationToken cancellationToken)
    {
        input.Top = Math.Min(input.Top ?? DefaultTop, MaxTop);

        var typeOfT = typeof(T);

        var sc = new ServiceCollection();

        return new ODataQueryOut<T>();
    }
}