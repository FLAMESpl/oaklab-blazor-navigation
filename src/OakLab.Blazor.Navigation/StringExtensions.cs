namespace OakLab.Blazor.Navigation;

internal static class StringExtensions
{
    internal static string ToStringOrEmpty<T>(this T? obj) => obj?.ToString() ?? string.Empty;
}
