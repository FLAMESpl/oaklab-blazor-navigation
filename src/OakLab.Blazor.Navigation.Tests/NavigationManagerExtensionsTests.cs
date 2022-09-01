using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace OakLab.Blazor.Navigation.Tests;

public class NavigationManagerExtensionsTests
{
    private readonly TestNavigationManager navigationManager = new();

    [Fact]
    public void CanNavigateToPageByTypeWithoutParameters()
    {
        navigationManager.NavigateTo<TestPageWithoutParameters>();
        navigationManager.LastNavigatedUri.Should().Be(TestPageWithoutParameters.RouteTemplate);
    }

    [Fact]
    public void CannotNavigateToPageByTypeWithoutMandatoryParameter()
    {
        navigationManager.Invoking(x => x.NavigateTo<TestPageWithParameters>()).Should().Throw<RouteConstructionException>();
    }

    [Fact]
    public void CanNavigateToPageByTypeWithMandatoryParameters()
    {
        navigationManager.NavigateTo<TestPageWithParameters>("PARAM1", "PARAM2");
        navigationManager.LastNavigatedUri.Should().Be(string.Format(TestPageWithParameters.RouteTemplate, "PARAM1", "PARAM2"));
    }

    [Fact]
    public void CanNavigateToPageByTypeWithMandatoryParametersAsEnumerable()
    {
        navigationManager.NavigateTo<TestPageWithParameters>(new[] { "PARAM1", "PARAM2" }.AsEnumerable());
        navigationManager.LastNavigatedUri.Should().Be(string.Format(TestPageWithParameters.RouteTemplate, "PARAM1", "PARAM2"));
    }

    [Fact]
    public void CanNavigateToPageByTypeWithQueryParametersFromDictionary()
    {
        navigationManager.NavigateWithQueryParametersTo<TestPageWithoutParameters>(
            new Dictionary<string, object> { ["QueryParameter"] = 1 });

        navigationManager.LastNavigatedUri.Should().Be($"{TestPageWithoutParameters.RouteTemplate}?QueryParameter=1");
    }

    [Fact]
    public void CanNavigateToPageByTypeWithQueryParametersFromString()
    {
        navigationManager.NavigateWithQueryParametersTo<TestPageWithoutParameters>("?QueryParameter=1");
        navigationManager.LastNavigatedUri.Should().Be($"{TestPageWithoutParameters.RouteTemplate}?QueryParameter=1");
    }

    [Fact]
    public void CanNavigateToPageByTypeWithQueryParametersFromObject()
    {
        navigationManager.NavigateWithQueryParametersTo<TestPageWithoutParameters>(new { QueryParameter = 1 });
        navigationManager.LastNavigatedUri.Should().Be($"{TestPageWithoutParameters.RouteTemplate}?QueryParameter=1");
    }

    [Fact]
    public void CanNavigateToPageByTypeWithRouteAndQueryParameters()
    {
        navigationManager.NavigateWithQueryParametersTo<TestPageWithParameters>(
            new { QueryParameter = 1 },
            "PARAM1",
            "PARAM2");

        navigationManager.LastNavigatedUri.Should().Be(
            string.Format(TestPageWithParameters.RouteTemplate, "PARAM1", "PARAM2") + "?QueryParameter=1");
    }

    [Fact]
    public void CanNavigateToPageWithParametersUsingRouteObject()
    {
        var route = new TestRouteWithParametersBoundByConvention
        {
            Parameter1 = Guid.NewGuid(),
            Parameter2 = "TEST",
            QueryParameter = 1
        };

        navigationManager.NavigateTo(route);
        navigationManager.LastNavigatedUri.Should().Be(
            string.Format(TestPageWithParameters.RouteTemplate, route.Parameter1, route.Parameter2) + "?QueryParameter=1");
    }

    [Fact]
    public void CanNavigateToPageWithoutRouteParametersUsingRouteObject()
    {
        var route = new TestRouteWithoutRouteParametersBoundByConvention
        {
            QueryParameter1 = 1
        };

        navigationManager.NavigateTo(route);
        navigationManager.LastNavigatedUri.Should().Be($"{TestPageWithoutParameters.RouteTemplate}?QueryParameter1=1");
    }

    [Theory]
    [InlineData("https://www.example.com/page")]
    [InlineData("https://www.example.com/page/")]
    [InlineData("https://www.example.com/page?param=123")]
    [InlineData("https://www.example.com/page/?param=123")]
    public void CanGetCurrentRouteThatHasNoParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutAnyParametersBoundByConvention>();
        route.Should().NotBeNull();
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter1=123")]
    [InlineData("https://www.example.com/page/?QueryParameter1=123")]
    public void CanGetCurrentRouteThatHasIntegerQueryParameterWhenThereAreQueryParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter1.Should().Be(123);
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter2=1.23")]
    [InlineData("https://www.example.com/page/?QueryParameter2=1.23")]
    public void CanGetCurrentRouteThatHasFloatQueryParameterWhenThereAreQueryParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter2.Should().Be(1.23F);
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter3=1.23")]
    [InlineData("https://www.example.com/page/?QueryParameter3=1.23")]
    public void CanGetCurrentRouteThatHasDecimalQueryParameterWhenThereAreQueryParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter3.Should().Be(1.23M);
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter4=07D54594-5FB8-42D4-B9F9-32ED3941EA1A")]
    [InlineData("https://www.example.com/page/?QueryParameter4=07d54594-5fb8-42d4-b9f9-32ed3941ea1a")]
    public void CanGetCurrentRouteThatHasGuidQueryParameterWhenThereAreQueryParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter4.Should().Be("07d54594-5fb8-42d4-b9f9-32ed3941ea1a");
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter5", true)]
    [InlineData("https://www.example.com/page?QueryParameter5=true", true)]
    [InlineData("https://www.example.com/page?QueryParameter5=True", true)]
    [InlineData("https://www.example.com/page?QueryParameter5=TRUE", true)]
    [InlineData("https://www.example.com/page?QueryParameter5=false", false)]
    [InlineData("https://www.example.com/page?QueryParameter5=False", false)]
    [InlineData("https://www.example.com/page?QueryParameter5=FALSE", false)]
    public void CanGetCurrentRouteThatHasBoolQueryParameterWhenThereAreQueryParameters(string uri, bool expectedValue)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter5.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("https://www.example.com/page")]
    [InlineData("https://www.example.com/page?")]
    [InlineData("https://www.example.com/page/")]
    [InlineData("https://www.example.com/page/?")]
    public void CanGetCurrentRouteThatHasQueryParameterWhenThereAreNoQueryParameters(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        var route = navigation.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>();
        route.Should().NotBeNull();
        route.QueryParameter1.Should().BeNull();
        route.QueryParameter2.Should().BeNull();
        route.QueryParameter3.Should().BeNull();
        route.QueryParameter4.Should().BeNull();
        route.QueryParameter5.Should().BeNull();
    }

    [Theory]
    [InlineData("https://www.example.com/page?QueryParameter1=test")]
    [InlineData("https://www.example.com/page/?QueryParameter1=test")]
    [InlineData("https://www.example.com/page?QueryParameter1=1.6")]
    [InlineData("https://www.example.com/page/?QueryParameter1=1.6")]
    [InlineData("https://www.example.com/page?QueryParameter1=1,6")]
    [InlineData("https://www.example.com/page/?QueryParameter1=1,6")]
    public void WhenUriHasParametersNotParsableToRoutesQueryParameterThenRouteConstructionExceptionIsThrown(string uri)
    {
        var navigation = new TestNavigationManager(uri);
        navigation
            .Invoking(x => x.GetCurrentRoute<TestRouteWithoutRouteParametersBoundByConvention>())
            .Should()
            .Throw<RouteConstructionException>()
            .Where(x => x.Message.Contains("Cannot convert value"));
    }
}
