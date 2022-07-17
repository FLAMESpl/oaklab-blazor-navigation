using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace OakLab.Blazor.Navigation.Tests;

public class QueryStringTests
{
    [Fact]
    public void CanConstructEmptyQueryParametersFromEmptyObject()
    {
        QueryString
            .GetQueryStringParametersFrom(new object())
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void CanConstructEmptyQueryParametersFromNonEmptyObjectWithNullParameter()
    {
        QueryString
            .GetQueryStringParametersFrom(new { ParameterName = null as object })
            .Should()
            .BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1f)]
    [InlineData("value")]
    public void CanConstructNonEmptyQueryParametersFromNonEmptyObjectWithNonNullParameter(object parameter)
    {
        QueryString
            .GetQueryStringParametersFrom(new { ParameterName = parameter })
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeEquivalentTo(KeyValuePair.Create("ParameterName", parameter));
    }

    [Fact]
    public void CanBuildEmptyQueryStringFromEmptyQueryParameters()
    {
        QueryString
            .Build(new Dictionary<string, object>())
            .Should()
            .BeEmpty();
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(1.1f, "1.1")]
    [InlineData("value", "value")]
    [InlineData("=?%", "%3d%3f%25")]
    public void CanBuildNonEmptyQueryStringFromSingleQueryParameter(object parameterValue, string stringValue)
    {
        QueryString
            .Build(new Dictionary<string, object>
            {
                ["Name"] = parameterValue
            })
            .Should()
            .Be($"?Name={stringValue}");
    }

    [Fact]
    public void CanBuildNonEmptyQueryStringFromTwoQueryParameters()
    {
        QueryString
            .Build(new Dictionary<string, object>
            {
                ["First"] = true,
                ["Second"] = false
            })
            .Should()
            .Be("?First=True&Second=False");
    }
}
