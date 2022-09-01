using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation.Tests;

public class TestNavigationManager : NavigationManager
{
    public TestNavigationManager(string uri = "https://www.google.com/")
    {
        Initialize(uri, uri);
    }

    public string? LastNavigatedUri { get; private set; }

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
        LastNavigatedUri = uri;
    }
}
