using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace OakLab.Blazor.Navigation.Tests;

public class RouteTests
{
    [Fact]
    public void CanGetParametersFromRouteObjectThatContainsParametersBoundByConvention()
    {
        var queryParameterName = nameof(TestRouteWithParametersBoundByConvention.QueryParameter);
        var route = new TestRouteWithParametersBoundByConvention
        {
            Parameter1 = Guid.NewGuid(),
            Parameter2 = "TEST",
            QueryParameter = 100
        };

        route.GetRouteParameters().Should().BeEquivalentTo(new object[] { route.Parameter1, route.Parameter2 });
        route.GetQueryParameters().Should().BeEquivalentTo(new[] { KeyValuePair.Create(queryParameterName, route.QueryParameter) });
    }

    [Fact]
    public void CanGetEmptyRouteParametersFromRouteObjectThatDoesNotContainRouteParametersButIsBoundByConvention()
    {
        var queryParameterName = nameof(TestRouteWithoutParametersBoundByConvention.QueryParameter);
        var route = new TestRouteWithoutParametersBoundByConvention
        {
            QueryParameter = 100
        };

        route.GetRouteParameters().Should().BeEmpty();
        route.GetQueryParameters().Should().BeEquivalentTo(new[] { KeyValuePair.Create(queryParameterName, route.QueryParameter) });
    }

    [Fact]
    public void CanGetEmptyQueryParametersWhenRouteDoesNotContainOptionalQueryParameterBoundByConvention()
    {
        var route = new TestRouteWithoutParametersBoundByConvention
        {
            QueryParameter = null
        };

        route.GetQueryParameters().Should().BeEquivalentTo(Enumerable.Empty<KeyValuePair<string, object>>());
    }

    [Fact]
    public void CanGetEmptyParametersFromRouteObjectThatDoesNotContainAnyParametersButIsBoundByConvention()
    {
        var route = new TestRouteWithoutAnyParametersBoundByConvention();

        route.GetRouteParameters().Should().BeEmpty();
        route.GetQueryParameters().Should().BeEquivalentTo(Enumerable.Empty<KeyValuePair<string, object>>());
    }

    [Fact]
    public void CanGetParametersFromRouteThatContainsParametersButAreNotBoundByConvention()
    {
        var route = new TestRouteWithParametersAndOverridenParametersBinding
        {
            Parameter1 = Guid.NewGuid(),
            Parameter2 = "TEST",
            QueryParameter = 100
        };

        route.GetRouteParameters().Should().BeEquivalentTo(new object[]
        {
            TestRouteWithParametersAndOverridenParametersBinding.HardcodedParameter1,
            TestRouteWithParametersAndOverridenParametersBinding.HardcodedParameter2
        });

        route.GetQueryParameters().Should().BeEquivalentTo(new[]
        {
            KeyValuePair.Create(
                nameof(TestRouteWithParametersAndOverridenParametersBinding.QueryParameter),
                TestRouteWithParametersAndOverridenParametersBinding.HardcodedQueryParameter)
        });
    }

    [Fact]
    public void CanGetParametersFromRouteNotContainingParametersButWithOverriddenBinding()
    {
        var route = new TestRouteWithoutParametersAndOverridenParametersBinding();


        route.GetRouteParameters().Should().BeEquivalentTo(new object[]
        {
            TestRouteWithoutParametersAndOverridenParametersBinding.HardcodedParameter1,
            TestRouteWithoutParametersAndOverridenParametersBinding.HardcodedParameter2
        });

        route.GetQueryParameters().Should().BeEquivalentTo(new[]
        {
            KeyValuePair.Create(
                TestRouteWithoutParametersAndOverridenParametersBinding.QueryParameterName,
                TestRouteWithoutParametersAndOverridenParametersBinding.HardcodedQueryParameter)
        });
    }
}
