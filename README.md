# Blazor Navigation

![Build](https://github.com/FLAMESpl/oaklab-blazor-navigation/actions/workflows/master-build.yml/badge.svg)
![Nuget](https://img.shields.io/nuget/v/OakLab.Blazor.navigation.svg?style=flat-round&label=nuget)

Get rid of magic url strings and navigate to page classes.

```csharp
navigationManager.NavigateTo<HomePage>();
```

No overrides to routing mechanism, just stateless extensions to navigation manager.

## Usage

### Route parameters

Supposedly you have route parameters on your page - `/blogs/{BlogName}/posts/{PostId}/comments`.

Route is parsed and argument templates are found which requires exactly to arguments to be passed to the function.

```csharp
navigationManager.NavigateTo<CommentsPage>(blogName, postId);
```

Route arguments are matched by their order in route template.

### Query parameters

There are three options to pass query parameters.

**With plain string**

```csharp
navigationManager.NavigateWithQueryParametersTo<DashboardPage>("?From=2022-01-01&ShowInactive=true");
```

**With enumeration of key value pairs**

```csharp
navigationManager.NavigateWithQueryParametersTo<DashboarPage>(new Dictionary<string, object>()
{
    ["From"] = "2022-01-01",
    ["ShowInactive"] = true
});
```

Type must derive from `IEnumerable<KeyValuePair<string, object>>`.

**With plain old CLR object**

```csharp
navigationManager.NavigateWithQueryParametersTo<DashboardPage>(new
{
    From = new DateOnly(2022, 1, 1), // all parameters are formatted with invariant culture
    ShowInactive = true
});
```

**With route parameters too**

Route parameters goes after query parameters.

```csharp
navigationManager.NavigateWithQueryParametersTo<DashboardPage>(queryParameters, routeParameter1, routeParameter2);
```

### Route objects

This package introduces abstract base class `Route<>`. Every route class directs to single page which is specified by generic argument.
One can navigate like:

```csharp
navigationManager.NavigateTo(dashboardRoute);
```

where `dashboardRoute` is instance of `Route<DashboardPage>` inheritor.

Extension method takes all public properties and invokes two virtual methods for corresponding parameters - `GetRouteParameters()` and `GetQueryParameters()`.
When method is not overriden then it reads arguments from public properties of the class, route parameters are matched by placeholder in route template
and remaining are treated as query parameters.

For example assume route template `/blogs/{BlogName}/posts/{PostId}/comments` and route object

```csharp
class CommentsRoute : Route<CommentsPage>
{
    public string BlogName { get; set; }
    public Guid PostId { get; set; }
    public PageSize PageSize { get; set; }
    public PageNumber PageNumber { get; set; }
}
```

In this scenario `BlogName` and `PostId` will be bound to route parameters where `PageSize` and `PageNumber` to query parameters.
Type of properties generally doesn't matter, there is only `.ToString()` invoked on them to construct a route,
or in case it implements `IFormattable` then `.ToString(null, CultureInfo.InvariantCulture)`.

To prevent property from binding annotate it with `[RouteIgnore]` attribute.



