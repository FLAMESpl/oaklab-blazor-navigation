# Changelog

## 0.2.0

* Changed method of type conversion of parameters to type converters obtained from type's descriptor.
* Added `SetParameters(Uri)`, `GetQueryParametersFromUri(Uri)` and `GetRouteParametersFromUri(Uri)`. `SetParameters(Uri)` can be invoked or overridden to bind parameters from uri, other methods extracts said parameters from uri.
* Added `GetCurrentRoute<TRoute>()` extension method to `NavigationManager`. It creates new instance of specified route object and uses `SetParameters(Uri)` to bind parameters from navigation's manager current URI.

## 0.1.0

* Initial release.
