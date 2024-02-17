using Azure.Core;

namespace Share.Helpers;

public static class StringExtensions {
    public static string? Clean(this string? value)
    {
        // Check if the string is null
        return value == null
            ? ""
            : value.Trim().ToUpper();
    }
}