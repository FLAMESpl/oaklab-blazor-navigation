using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation;

/// <summary>
/// Can be used to navigate to page specified by parameter <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Page component to which inheritor of this class will navigate.</typeparam>
public abstract class Route<T> where T : ComponentBase
{
    private readonly RouteDefinition<T> definition;

    /// <summary>
    /// Creates new instance of <see cref="Route{T}"/> inheritor.
    /// </summary>
    protected Route()
    {
        definition = RouteDefinition<T>.Get(GetType());
    }

    /// <summary>
    /// Returns sequence of route parameters of <typeparamref name="T"/> page component. Order of parameters in returned sequence
    /// must match with order of parameters in route template.
    /// </summary>
    /// <remarks>
    /// Base implementation will return values of all public properties that are not annotated with
    /// <see cref="RouteIgnoreAttribute"/> attribute and match placeholder names in route template.
    /// Order of parameters matches its order in route template placeholders.
    /// </remarks>
    /// <returns>Sequence of route parameters.</returns>
    public virtual IEnumerable<object> GetRouteParameters() => definition.GetRouteParametersFrom(this);

    /// <summary>
    /// Returns query parameters for <typeparamref name="T"/> page component. Parameters can have any of three following forms:
    /// <para>1. Raw query string in form of <see cref="string"/>, eg. ?From=2022-01-01&#38;ShowInactive=true</para>
    /// <para>2. Sequence implementing <see cref="T:IEnumerable{KeyValuePair{string, object}}"/> where key is parameter name and value is parameter value.</para>
    /// <para>3. Plain old CLR object where its public properties' names are parameter names and its values are parameter values.</para>
    /// Base implementation will return values of all public properties that are not annotated with
    /// <see cref="RouteIgnoreAttribute"/> attribute and do not match placeholder names in route template.
    /// </summary>
    /// <returns>Query parameters.</returns>
    public virtual object GetQueryParameters() => definition.GetQueryParametersFrom(this);
}
