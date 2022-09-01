using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace OakLab.Blazor.Navigation;

internal class RouteTemplate
{
    internal string RawString { get; }
    internal IReadOnlyList<string> NonParameterTokens { get; }
    internal IReadOnlyList<string> ParameterNames { get; }

    public RouteTemplate(string template)
    {
        RawString = template;
        (NonParameterTokens, ParameterNames) = ParseTemplate(template);
    }

    public string Construct(
        IReadOnlyList<string> parameters,
        string queryString)
    {
        if (parameters.Count != NonParameterTokens.Count && parameters.Count + 1 != NonParameterTokens.Count)
            throw new RouteConstructionException("Input parameters count does not match one in route template.");

        var builder = new StringBuilder();
        var firstToken = NonParameterTokens[0];

        // remove trailing '/' unless it is root token with single '/' character
        if (NonParameterTokens.Count != 1 || firstToken != "/")
        {
            for (var i = 0; i < NonParameterTokens.Count; i++)
            {
                var token = NonParameterTokens[i];

                builder.Append(NonParameterTokens.Count - 1 == i
                    ? token[..^1]
                    : token);

                if (parameters.Count > i)
                    builder.Append(parameters[i]);
            }
        }
        else
        {
            builder.Append(firstToken);
        }

        return builder.Append(queryString).ToString();
    }

    private static (IReadOnlyList<string> Tokens, IReadOnlyList<string> Parameters) ParseTemplate(string template)
    {
        template = template.NormalizePath();
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

        var lastToken = template[previousPosition..];
        if (!string.IsNullOrEmpty(lastToken))
            tokens.Add(lastToken);

        return (tokens, parameterNames);
    }
}

internal static class RouteTemplate<T>
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
