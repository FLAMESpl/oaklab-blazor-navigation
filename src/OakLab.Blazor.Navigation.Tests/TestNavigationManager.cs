using Microsoft.AspNetCore.Components;

namespace OakLab.Blazor.Navigation.Tests;

public class TestNavigationManager : NavigationManager
{
    private const string dummyUri = "https://www.google.com/";

    public string? LastNavigatedUri { get; private set; }

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
        LastNavigatedUri = uri;
    }

    protected override void EnsureInitialized()
    {
        Initialize(dummyUri, dummyUri);
    }
}
