

using JetBrains.Annotations;



namespace Share.DbContracts;

[UsedImplicitly]
public class ODataQueryIn {
    /// <example>10</example>
    public int? Top { get; set; }

    /// <example>0</example>
    public int? Skip { get; set; }

    /// <example>false</example>
    public bool Count { get; set; }

    // public bool Definition { get; set; }
    // public bool JustDefinition { get; set; }

    /// <example>id acs</example>
    public string? OrderBy { get; set; }

    public string? Select { get; set; }

    public string? Filter { get; set; }

    public string? Search { get; set; }
}