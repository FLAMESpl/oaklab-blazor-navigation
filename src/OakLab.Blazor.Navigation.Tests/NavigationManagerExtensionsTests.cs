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
        var route = new TestRouteWithoutParametersBoundByConvention
        {
            QueryParameter = 1
        };

        navigationManager.NavigateTo(route);
        navigationManager.LastNavigatedUri.Should().Be($"{TestPageWithoutParameters.RouteTemplate}?QueryParameter=1");
    }
}
