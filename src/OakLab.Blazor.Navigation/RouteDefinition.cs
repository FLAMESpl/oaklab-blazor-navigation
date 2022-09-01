using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation;

internal class RouteDefinition<TPage> where TPage : ComponentBase
{
    public IReadOnlyList<PropertyInfo> RouteProperties { get; }
    public IReadOnlyCollection<PropertyInfo> QueryProperties { get; }
    public RouteTemplate Template => RouteTemplate<TPage>.Get();

    public RouteDefinition(Type routeType)
    {
        var properties = routeType
            .GetProperties()
            .Where(p => p.GetCustomAttribute<RouteIgnoreAttribute>() is null)
            .ToDictionary(p => p.Name);

        RouteProperties = Template.ParameterNames
            .Select(name => properties.GetValueOrDefault(name))
            .Where(p => p is not null)
            .ToList()!;

        QueryProperties = properties.Values.Where(p => !Template.ParameterNames.Contains(p.Name)).ToList();
    }

    public IEnumerable<object> GetRouteParametersFrom(Route<TPage> route) =>
        RouteProperties.Select(x => x.GetValue(route))!;

    public IEnumerable<KeyValuePair<string, object>> GetQueryParametersFrom(Route<TPage> route) =>
        QueryString.GetQueryStringParametersFrom(route, QueryProperties);

    public static RouteDefinition<TPage> Get(Type routeType) => (RouteDefinition<TPage>) typeof(RouteDefinitionCache<,>)
        .MakeGenericType(typeof(TPage), routeType)
        .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)!
        .GetValue(null)!;
}

internal class RouteDefinitionCache<TPage, TRoute>
    where TPage : ComponentBase
    where TRoute : Route<TPage>
{
    public static RouteDefinition<TPage> Instance { get; } = new(typeof(TRoute));
}
