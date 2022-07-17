using FluentAssertions;
using System;
using Xunit;

namespace OakLab.Blazor.Navigation.Tests;

public class RouteTemplateTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/test")]
    [InlineData("/test1/test2")]
    public void UriWithoutArgumentsIsConstructedCorrectly(string template)
    {
        var route = new RouteTemplate(template);
        var uri = route.Construct(Array.Empty<string>(), string.Empty);
        uri.Should().Be(template);
    }

    [Theory]
    [InlineData("/{Id:guid}", "/00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    [InlineData("/test/{Id:guid}", "/test/00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    [InlineData("/test1/{Id:guid}/test2", "/test1/00000000-0000-0000-0000-000000000000/test2", "00000000-0000-0000-0000-000000000000")]
    [InlineData("/test1/{Id:guid}/test2/{Code}", "/test1/00000000-0000-0000-0000-000000000000/test2/ZXC", "00000000-0000-0000-0000-000000000000", "ZXC")]
    public void UriWithArgumentsIsConstructedCorrectly(
        string template,
        string expectedUri,
        params string[] parameters)
    {
        var route = new RouteTemplate(template);
        var uri = route.Construct(parameters, string.Empty);
        uri.Should().Be(expectedUri);
    }

}
