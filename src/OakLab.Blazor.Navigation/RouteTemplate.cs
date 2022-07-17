using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OakLab.Blazor.Navigation;

internal class RouteTemplate
{
    private readonly IReadOnlyList<string> templateTokens;

    internal IReadOnlyList<string> ParameterNames { get; }

    public RouteTemplate(string template)
    {
        (templateTokens, ParameterNames) = ParseTemplate(template);
    }

    public string Construct(
        IReadOnlyList<string> parameters,
        string queryString)
    {
        if (parameters.Count != templateTokens.Count && parameters.Count + 1 != templateTokens.Count)
            throw new RouteConstructionException("Input parameters count does not match one in route template.");

        var builder = new StringBuilder();

        for (var i = 0; i < templateTokens.Count; i++)
        {
            builder.Append(templateTokens[i]);

            if (parameters.Count > i)
                builder.Append(parameters[i]);
        }

        return builder.Append(queryString).ToString();
    }

    private static (IReadOnlyList<string> Tokens, IReadOnlyList<string> Parameters) ParseTemplate(
        string template)
    {
        var tokens = new List<string>();
        var parameterNames = new List<string>();
        var previousPosition = 0;
        var currentPosition = template.IndexOf('{');

        while (currentPosition >= 0)
        {
            if (currentPosition > previousPosition)
                tokens.Add(template[previousPosition..currentPosition]);

            var closingPosition = template.IndexOf('}', currentPosition + 1);
            var parameterTokens = template[(currentPosition + 1)..closingPosition].Split(':');

            if (parameterTokens.Length > 0)
                parameterNames.Add(parameterTokens[0]);

            previousPosition = closingPosition + 1;
            currentPosition = template.Length < previousPosition
                ? -1
                : template.IndexOf('{', previousPosition);
        }

        tokens.Add(template[previousPosition..]);

        return (tokens, parameterNames);
    }
}

internal class RouteTemplate<T>
{
    private static readonly RouteTemplate? instance;

    static RouteTemplate()
    {
        instance = typeof(T).GetCustomAttribute<RouteAttribute>() is { } attribute
            ? new RouteTemplate(attribute.Template)
            : null;
    }

    public static RouteTemplate Get() => instance ??
        throw new ArgumentException($"Type `{typeof(T).FullName}` is not annotated with Route attribute.");
}
