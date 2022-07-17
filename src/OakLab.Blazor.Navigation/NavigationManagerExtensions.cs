using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace OakLab.Blazor.Navigation;

public static class NavigationManagerExtensions
{
    public static void NavigateTo<T>(
        this NavigationManager navigationManager,
        Route<T> route,
        bool forceLoad = false) where T : ComponentBase
    {
        NavigateWithQueryParametersTo<T>(
            navigationManager: navigationManager,
            queryParameters: route.GetQueryParameters(),
            parameters: route.GetRouteParameters(),
            forceLoad: forceLoad);
    }

    public static void NavigateTo<T>(
        this NavigationManager navigationManager,
        params object[] parameters) where T : ComponentBase
    {
        NavigateWithQueryParametersTo<T>(
            navigationManager: navigationManager,
            queryParameters: string.Empty,
            parameters: parameters,
            forceLoad: false);
    }

    public static void NavigateTo<T>(
        this NavigationManager navigationManager,
        IEnumerable<object> parameters,
        bool forceLoad = false) where T : ComponentBase
    {
        NavigateWithQueryParametersTo<T>(
            navigationManager: navigationManager,
            queryParameters: string.Empty,
            parameters: parameters,
            forceLoad: forceLoad);
    }

    public static void NavigateWithQueryParametersTo<T>(
        this NavigationManager navigationManager,
        object? queryParameters,
        params object[] parameters) where T : ComponentBase
    {
        NavigateWithQueryParametersTo<T>(
            navigationManager: navigationManager,
            queryParameters: queryParameters,
            parameters: parameters,
            forceLoad: false);
    }

    public static void NavigateWithQueryParametersTo<T>(
        this NavigationManager navigationManager,
        object? queryParameters,
        IEnumerable<object> parameters,
        bool forceLoad = false) where T : ComponentBase
    {
        var queryString = GetQueryString(queryParameters);
        var uri = RouteTemplate<T>.Get().Construct(parameters.Select(x => x.ToStringOrEmpty()).ToList(), queryString);
        navigationManager.NavigateTo(uri, forceLoad);
    }

    private static string GetQueryString(object? parameters)
    {
        if (parameters is string queryParameters)
            return queryParameters;
        else if (parameters is null)
            return string.Empty;
        else
            return QueryString.Build(parameters as IEnumerable<KeyValuePair<string, object>> ??
                                     QueryString.GetQueryStringParametersFrom(parameters));
    }
}
