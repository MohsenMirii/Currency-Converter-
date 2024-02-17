

using JetBrains.Annotations;



namespace Share.DbContracts;

[PublicAPI]
public class ODataQueryOut<T> : HasJsonOutputSettings {
    public int? Count { get; set; }

    public List<T> Data { get; set; } = null!;

    // public List<FieldDefinition>? Definitions { get; set; }
}