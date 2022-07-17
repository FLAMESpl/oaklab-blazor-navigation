using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation.Tests;

[Route("/page/{Parameter1:guid}/something/{Parameter2}")]
public class TestPageWithParameters : ComponentBase
{
    public const string RouteTemplate = "/page/{0}/something/{1}";
}

[Route("/page")]
public class TestPageWithoutParameters : ComponentBase
{
    public const string RouteTemplate = "/page";
}

public class TestRouteWithParametersBoundByConvention : Route<TestPageWithParameters>
{
    public Guid Parameter1 { get; set; }
    public string Parameter2 { get; set; } = string.Empty;
    public int QueryParameter { get; set; }
}

public class TestRouteWithoutParametersBoundByConvention : Route<TestPageWithoutParameters>
{
    public int? QueryParameter { get; set; }
}

public class TestRouteWithoutAnyParametersBoundByConvention : Route<TestPageWithoutParameters>
{
}

public class TestRouteWithParametersAndOverridenParametersBinding : Route<TestPageWithParameters>
{
    public static readonly Guid HardcodedParameter1 = Guid.NewGuid();
    public const string HardcodedParameter2 = "Hardcoded";
    public const int HardcodedQueryParameter = 13456789;

    public Guid Parameter1 { get; set; }
    public string Parameter2 { get; set; } = string.Empty;
    public int QueryParameter { get; set; }

    public override IEnumerable<object> GetRouteParameters()
    {
        yield return HardcodedParameter1;
        yield return HardcodedParameter2;
    }

    public override object GetQueryParameters()
    {
        return new[] { KeyValuePair.Create(nameof(QueryParameter), HardcodedQueryParameter) };
    }
}

public class TestRouteWithoutParametersAndOverridenParametersBinding : Route<TestPageWithParameters>
{
    public static readonly Guid HardcodedParameter1 = Guid.NewGuid();
    public const string HardcodedParameter2 = "Hardcoded";
    public const string HardcodedQueryParameter = "Query Hardcoded";
    public const string QueryParameterName = "QueryParameter";

    public override IEnumerable<object> GetRouteParameters()
    {
        yield return HardcodedParameter1;
        yield return HardcodedParameter2;
    }

    public override object GetQueryParameters()
    {
        return new[] { KeyValuePair.Create(QueryParameterName, HardcodedQueryParameter) };
    }
}
