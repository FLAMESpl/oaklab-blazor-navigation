using System;

namespace OakLab.Blazor.Navigation;

internal static class TypeExtensions
{
    public static object? GetDefaultValue(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
}
