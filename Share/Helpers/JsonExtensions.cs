

using System.Text.Json;
using System.Text.Json.Serialization;



namespace Share.Helpers;

public static class JsonExtensions {
    private static readonly JsonSerializerOptions DefaultOptions = new(JsonSerializerDefaults.Web);

    static JsonExtensions()
    {
        DefaultOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static string ToJsonString(this object obj)
    {
        return JsonSerializer.Serialize(obj, DefaultOptions);
    }

    public static string ToJsonStringForDeveloper(this object obj)
    {
        return JsonSerializer.Serialize(obj,
                new JsonSerializerOptions
                {
                    WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            .Replace("\"", "'");
    }

    public static JsonDocument ToJsonDocument(this object obj)
    {
        return JsonDocument.Parse(obj.ToJsonString());
    }

    public static T FromJsonString<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultOptions)
               ?? throw new Exception("json deserialize returns null");
    }
}