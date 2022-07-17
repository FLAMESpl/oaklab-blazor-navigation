using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation;

internal class RouteDefinition<TPage> where TPage : ComponentBase
{
    private readonly IReadOnlyList<PropertyInfo> routeProperties;
    private readonly IReadOnlyCollection<PropertyInfo> queryProperties;

    public RouteDefinition(Type routeType)
    {
        var template = RouteTemplate<TPage>.Get();
        var properties = routeType
            .GetProperties()
            .Where(p => p.GetCustomAttribute<RouteIgnoreAttribute>() is null)
            .ToDictionary(p => p.Name);

        routeProperties = template.ParameterNames
            .Select(name => properties.GetValueOrDefault(name))
            .Where(p => p is not null)
            .ToList()!;

        queryProperties = properties.Values.Where(p => !template.ParameterNames.Contains(p.Name)).ToList();
    }

    public IEnumerable<object> GetRouteParametersFrom(Route<TPage> route) =>
        routeProperties.Select(x => x.GetValue(route))!;

    public IEnumerable<KeyValuePair<string, object>> GetQueryParametersFrom(Route<TPage> route) =>
        QueryString.GetQueryStringParametersFrom(route, queryProperties);

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
