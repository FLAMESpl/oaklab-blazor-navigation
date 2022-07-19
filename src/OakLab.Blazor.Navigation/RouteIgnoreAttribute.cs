using System;

namespace OakLab.Blazor.Navigation;

/// <summary>
/// Properties of <see cref="Route{T}"/> object annotated with this attribute will not bind to navigation uri.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RouteIgnoreAttribute : Attribute
{
}
