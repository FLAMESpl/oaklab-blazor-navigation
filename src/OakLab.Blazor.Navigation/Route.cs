using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation;

/// <summary>
/// Base interface for routes that can be used for navigation to pages.
/// </summary>
public interface IRoute
{
    /// <summary>
    /// Gets type of the page that this route navigates to.
    /// </summary>
    Type GetPageType();

    /// <summary>
    /// Returns sequence of route parameters of page component. Order of parameters in returned sequence
    /// must match with order of parameters in route template.
    /// </summary>
    /// <remarks>
    /// Base implementation will return values of all public properties that are not annotated with
    /// <see cref="RouteIgnoreAttribute"/> attribute and match placeholder names in route template.
    /// Order of parameters matches its order in route template placeholders.
    /// </remarks>
    /// <returns>Sequence of route parameters.</returns>
    IEnumerable<object> GetRouteParameters();

    /// <summary>
    /// Returns query parameters for page component. Parameters can have any of three following forms:
    /// <para>1. Raw query string in form of <see cref="string"/>, eg. ?From=2022-01-01&#38;ShowInactive=true</para>
    /// <para>2. Sequence implementing <see cref="T:IEnumerable{KeyValuePair{string, object}}"/> where key is parameter name and value is parameter value.</para>
    /// <para>3. Plain old CLR object where its public properties' names are parameter names and its values are parameter values.</para>
    /// Base implementation will return values of all public properties that are not annotated with
    /// <see cref="RouteIgnoreAttribute"/> attribute and do not match placeholder names in route template.
    /// </summary>
    /// <returns>Query parameters.</returns>
    object GetQueryParameters();

    /// <summary>
    /// Binds values from <paramref name="uri"/> to properties of this route object.
    /// Type (query and route) is determined basing on page's <see cref="RouteAttribute"/>.
    /// </summary>
    /// <param name="uri"><see cref="Uri"/> with parameters.</param>
    /// <exception cref="RouteConstructionException">
    /// This exception is thrown when either route does not match one in page's
    /// <see cref="RouteAttribute"/> or value of a parameter is not convertible to corresponding
    /// property of an object.
    /// </exception>
    void SetParameters(Uri uri);
}

/// <summary>
/// Can be used to navigate to page specified by parameter <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Page component to which inheritor of this class will navigate.</typeparam>
public abstract class Route<T> : IRoute where T : ComponentBase
{
    private readonly RouteDefinition<T> definition;

    /// <summary>
    /// Creates new instance of <see cref="Route{T}"/> inheritor.
    /// </summary>
    protected Route()
    {
        definition = RouteDefinition<T>.Get(GetType());
    }

    /// <inheritdoc cref="IRoute.GetPageType"/>
    public Type GetPageType() => typeof(T);

    /// <inheritdoc cref="IRoute.GetRouteParameters"/>
    public virtual IEnumerable<object> GetRouteParameters() => definition.GetRouteParametersFrom(this);


    /// <inheritdoc cref="IRoute.GetRouteParameters"/>
    public virtual object GetQueryParameters() => definition.GetQueryParametersFrom(this);

    /// <inheritdoc cref="IRoute.SetParameters"/>
    public virtual void SetParameters(Uri uri)
    {
        var routeParameters = GetRouteParametersFromUri(uri);
        var queryParameters = GetQueryParametersFromUri(uri);

        // These are really keys without values, but HttpUtility understands it as values without keys.
        var queryParametersWithoutKey =
            queryParameters.GetValues(null)?.ToHashSet() as IReadOnlySet<string> ?? ImmutableHashSet<string>.Empty;

        foreach (var routeProperty in definition.RouteProperties)
        {
            var value = ConvertValue(
                routeParameters[routeProperty.Name],
                routeProperty.PropertyType,
                routeProperty.Name);

            routeProperty.SetValue(this, value);
        }

        foreach (var queryProperty in definition.QueryProperties)
        {
            var textValue = queryParameters.GetValues(queryProperty.Name)?.LastOrDefault();
            object? value;

            if (textValue is not null)
            {
                value = ConvertValue(textValue, queryProperty.PropertyType, queryProperty.Name);
            }
            else if (queryParametersWithoutKey.Contains(queryProperty.Name))
            {
                value = true; // When there is no value for key it is commonly understand to be 'switch' parameter.
            }
            else
            {
                value = null;
            }

            queryProperty.SetValue(this, value ?? queryProperty.PropertyType.GetDefaultValue());
        }
    }

    /// <summary>
    /// Extracts route parameters from <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    /// <exception cref="RouteConstructionException">
    /// This exception is thrown when route does not match one in page's <see cref="RouteAttribute"/>.
    /// </exception>
    protected IReadOnlyDictionary<string, string> GetRouteParametersFromUri(Uri uri)
    {
        var parameters = new Dictionary<string, string>();
        var nonParameterTokens = definition.Template.NonParameterTokens;
        var definedParameters = definition.RouteProperties;
        var currentPosition = 0;
        var path = uri.AbsolutePath.NormalizePath();

        for (var i = 0; i < nonParameterTokens.Count; i++)
        {
            var moreParametersAhead = definedParameters.Count > i;
            var templateToken = nonParameterTokens[i];
            var pathTokenEnd = path.IndexOf('/', currentPosition + 1);
            var pathToken = pathTokenEnd > 0 && moreParametersAhead
                ? path[currentPosition..(pathTokenEnd + 1)]
                : path[currentPosition..];

            if (!pathToken.Equals(templateToken.Value, StringComparison.OrdinalIgnoreCase))
                throw GetPathNotMatchingTemplateException(path);

            if (!moreParametersAhead)
                break;

            var parameterStart = currentPosition + templateToken.Value.Length;

            if (nonParameterTokens.Count - 1 == i)
            {
                currentPosition = path.Length;
            }
            else
            {
                currentPosition = path.IndexOf('/', parameterStart);

                if (currentPosition < 0)
                    throw GetPathNotMatchingTemplateException(path);
            }

            parameters.Add(definedParameters[i].Name, path[parameterStart..currentPosition]);
        }

        return parameters;
    }

    /// <summary>
    /// Extracts query parameters from <paramref name="uri"/>.
    /// All parameters without values are under <see langword="null"/> key.
    /// </summary>
    /// <param name="uri"><see cref="Uri"/> with parameters.</param>
    /// <returns><see cref="NameValueCollection"/> of query parameters.</returns>
    protected NameValueCollection GetQueryParametersFromUri(Uri uri)
    {
        return HttpUtility.ParseQueryString(uri.Query);
    }

    private Exception GetPathNotMatchingTemplateException(string path) => new RouteConstructionException(
        $"Input path `{path}` does not match page's template `{definition.Template.RawString}`.");

    private static object? ConvertValue(string textValue, Type targetType, string propertyName)
    {
        try
        {
            return TypeDescriptor.GetConverter(targetType).ConvertFromInvariantString(textValue);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            throw new RouteConstructionException(
                $"Cannot convert value `{textValue}` to type `{targetType}` for property {propertyName}.",
                ex);
        }
    }
}
