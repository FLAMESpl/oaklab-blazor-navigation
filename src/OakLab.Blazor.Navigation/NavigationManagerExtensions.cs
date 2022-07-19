using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace OakLab.Blazor.Navigation;

/// <summary>
/// Extensions for <see cref="NavigationManager"/>.
/// </summary>
public static class NavigationManagerExtensions
{
    /// <summary>
    /// Navigates to page directed by route object using its route and query parameters.
    /// </summary>
    /// <param name="navigationManager">Instance of <see cref="NavigationManager"/>.</param>
    /// <param name="route">Instance of class inheriting <see cref="Route{T}"/>.</param>
    /// <param name="forceLoad">If true, bypasses client-side routing and forces the browser to load the new page from
    /// the server, whether or not the URI would normally be handled by the client-side router.</param>
    /// <typeparam name="T">Type of page component to be navigated to.</typeparam>
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

    /// <summary>
    /// Navigates to page specified in generic argument with given route parameters.
    /// </summary>
    /// <param name="navigationManager">Instance of <see cref="NavigationManager"/>.</param>
    /// <param name="parameters">Sequence of route parameters. Order in sequence determines position in route template.</param>
    /// <typeparam name="T">Type of page component to be navigated to.</typeparam>
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

    /// <summary>
    /// Navigates to page specified in generic argument with given route parameters.
    /// </summary>
    /// <param name="navigationManager">Instance of <see cref="NavigationManager"/>.</param>
    /// <param name="parameters">Sequence of route parameters. Order in sequence determines position in route template.</param>
    /// <param name="forceLoad">If true, bypasses client-side routing and forces the browser to load the new page from
    /// the server, whether or not the URI would normally be handled by the client-side router.</param>
    /// <typeparam name="T">Type of page component to be navigated to.</typeparam>
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

    /// <summary>
    /// Navigates to page specified in generic argument with given route and query parameters.
    /// </summary>
    /// <param name="navigationManager">Instance of <see cref="NavigationManager"/>.</param>
    /// <param name="queryParameters">
    /// Query parameters in any of three following forms:
    /// <para>1. Raw query string in form of <see cref="string"/>, eg. ?From=2022-01-01&#38;ShowInactive=true</para>
    /// <para>2. Sequence implementing <see cref="T:IEnumerable{KeyValuePair{string, object}}"/> where key is parameter name and value is parameter value.</para>
    /// <para>3. Plain old CLR object where its public properties' names are parameter names and its values are parameter values.</para>
    /// </param>
    /// <param name="parameters">Sequence of route parameters. Order in sequence determines position in route template.</param>
    /// <typeparam name="T">Type of page component to be navigated to.</typeparam>
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

    /// <summary>
    /// Navigates to page specified in generic argument with given route and query parameters.
    /// </summary>
    /// <param name="navigationManager">Instance of <see cref="NavigationManager"/>.</param>
    /// <param name="queryParameters">
    /// Query parameters in any of three following forms:
    /// <para>1. Raw query string in form of <see cref="string"/>, eg. ?From=2022-01-01&#38;ShowInactive=true</para>
    /// <para>2. Sequence implementing <see cref="T:IEnumerable{KeyValuePair{string, object}}"/> where key is parameter name and value is parameter value.</para>
    /// <para>3. Plain old CLR object where its public properties' names are parameter names and its values are parameter values.</para>
    /// </param>
    /// <param name="parameters">Sequence of route parameters. Order in sequence determines position in route template.</param>
    /// <param name="forceLoad">If true, bypasses client-side routing and forces the browser to load the new page from
    /// the server, whether or not the URI would normally be handled by the client-side router.</param>
    /// <typeparam name="T">Type of page component to be navigated to.</typeparam>
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
