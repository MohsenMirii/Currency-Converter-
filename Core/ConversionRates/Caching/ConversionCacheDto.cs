namespace Core.ConversionRates.Caching;

public abstract class ConversionCacheDto {
    public long Id { get; set; }
    public string SourceCurrency { get; set; } = null!;
    public string DestinationCurrency { get; set; } = null!;
    public double Rate { get; set; }
}