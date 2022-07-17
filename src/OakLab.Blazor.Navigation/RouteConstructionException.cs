using System;

namespace OakLab.Blazor.Navigation;

public class RouteConstructionException : Exception
{
    public RouteConstructionException(string message) : base(message)
    {
    }
}
