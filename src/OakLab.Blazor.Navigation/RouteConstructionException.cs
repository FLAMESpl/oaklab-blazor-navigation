using System;

namespace OakLab.Blazor.Navigation;

/// <summary>
/// Exception indicating some errors during construction of route from its template and given arguments.
/// </summary>
public class RouteConstructionException : Exception
{
    internal RouteConstructionException(string message) : base(message)
    {
    }
}
