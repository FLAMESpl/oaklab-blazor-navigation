namespace OakLab.Blazor.Navigation.Sample.Pages;

public class Page3Route : Route<Page3>
{
    public string Parameter { get; }
    public int? QueryParameter { get; }

    public Page3Route(string parameter, int? queryParameter)
    {
        Parameter = parameter;
        QueryParameter = queryParameter;
    }
}
