using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace OakLab.Blazor.Navigation;

internal static class QueryString
{
    public static string Build(IEnumerable<KeyValuePair<string, object>> parameters)
    {
        using var enumerator = parameters.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var builder = new StringBuilder("?");
            AppendParameter(builder, enumerator.Current);

            while (enumerator.MoveNext())
            {
                builder.Append('&');
                AppendParameter(builder, enumerator.Current);
            }

            return builder.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    public static IEnumerable<KeyValuePair<string, object>> GetQueryStringParametersFrom(object @object)
    {
        return GetQueryStringParametersFrom(@object, @object.GetType().GetProperties());
    }

    public static IEnumerable<KeyValuePair<string, object>> GetQueryStringParametersFrom(
        object @object,
        IEnumerable<PropertyInfo> properties)
    {
        return properties
            .Select(p => KeyValuePair.Create(p.Name, p.GetValue(@object)))
            .Where(x => x.Value is not null)!;
    }

    private static void AppendParameter(StringBuilder builder, KeyValuePair<string, object> pair)
    {
        if (string.IsNullOrWhiteSpace(pair.Key))
            throw new InvalidOperationException("Parameter name cannot be empty.");

        var value = pair.Value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : pair.Value.ToString();

        builder.Append(pair.Key).Append('=').Append(HttpUtility.UrlEncode(value));
    }
}
