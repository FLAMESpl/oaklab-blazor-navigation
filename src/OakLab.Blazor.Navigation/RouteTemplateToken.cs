namespace OakLab.Blazor.Navigation;

internal readonly record struct RouteTemplateToken(string Value, int SeparatorCount)
{
    public override string ToString() => Value;
}
