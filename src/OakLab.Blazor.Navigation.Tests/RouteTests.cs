using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace OakLab.Blazor.Navigation.Tests;

public class RouteTests
{
    private readonly ITestOutputHelper testOutput;

    public RouteTests(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }

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
        var queryParameterName = nameof(TestRouteWithoutRouteParametersBoundByConvention.QueryParameter1);
        var route = new TestRouteWithoutRouteParametersBoundByConvention
        {
            QueryParameter1 = 100
        };

        route.GetRouteParameters().Should().BeEmpty();
        route.GetQueryParameters().Should().BeEquivalentTo(new[] { KeyValuePair.Create(queryParameterName, route.QueryParameter1) });
    }

    [Fact]
    public void CanGetEmptyQueryParametersWhenRouteDoesNotContainOptionalQueryParameterBoundByConvention()
    {
        var route = new TestRouteWithoutRouteParametersBoundByConvention
        {
            QueryParameter1 = null
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

    [Theory]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/pag")]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/pag/")]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/pagee")]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/page/p")]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/page/p/")]
    [InlineData(typeof(TestRouteWithoutRouteParametersBoundByConvention), "https://www.example.com/page/param")]
    [InlineData(typeof(TestRouteWithParametersBoundByConvention), "https://www.example.com/page")]
    [InlineData(typeof(TestRouteWithParametersBoundByConvention), "https://www.example.com/5e5ba5cf-210b-435d-9e72-c67c3ce8e778/something/param")]
    [InlineData(typeof(TestRouteWithParametersBoundByConvention), "https://www.example.com/page/5e5ba5cf-210b-435d-9e72-c67c3ce8e778/something")]
    [InlineData(typeof(TestRouteWithParametersBoundByConvention), "https://www.example.com/page/5e5ba5cf-210b-435d-9e72-c67c3ce8e778/something/param/more")]
    [InlineData(typeof(TestRouteWithParametersBoundByConvention), "https://www.example.com/page/5e5ba5cf-210b-435d-9e72-c67c3ce8e778/something2/param")]
    public void IfPathDoesNotMatchTemplateWhenBindingRouteThenRouteConstructionExceptionIsThrown(
        Type routeType,
        string uri)
    {
        testOutput.WriteLine(uri);

        var route = (IRoute)Activator.CreateInstance(routeType)!;
        route
            .Invoking(x => x.SetParameters(new Uri(uri)))
            .Should()
            .Throw<RouteConstructionException>()
            .Where(x => x.Message.Contains("does not match page's template"));
    }

    [Fact]
    public void SettingPropertiesForUriWithoutParametersToAlreadyInitializedObjectOverwritesExistingValueWithDefault()
    {
        var route = new TestRouteWithParametersBoundByConvention { QueryParameter = 10 };
        route.SetParameters(new Uri("https://www.example.com/page/353E21D2-F7AD-44E0-B214-869E52228465/something/test"));
        route.QueryParameter.Should().Be(0);
    }

    [Fact]
    public void SettingPropertiesForUriWithoutParametersToAlreadyInitializedObjectOverwritesExistingValueWithNull()
    {
        var route = new TestRouteWithoutRouteParametersBoundByConvention { QueryParameter1 = 10, QueryParameter6 = "x" };
        route.SetParameters(new Uri("https://www.example.com/page"));
        route.QueryParameter1.Should().BeNull();
        route.QueryParameter6.Should().BeNull();
    }
}
