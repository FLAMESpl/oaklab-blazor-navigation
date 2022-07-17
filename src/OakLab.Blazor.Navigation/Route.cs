using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation;

public abstract class Route<T> where T : ComponentBase
{
    private readonly RouteDefinition<T> definition;

    protected Route()
    {
        definition = RouteDefinition<T>.Get(GetType());
    }

    public virtual IEnumerable<object> GetRouteParameters() => definition.GetRouteParametersFrom(this);
    public virtual object GetQueryParameters() => definition.GetQueryParametersFrom(this);
}
