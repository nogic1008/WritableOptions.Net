namespace XamarinExample;

public partial class AppOption
{
    public DateTime LastChanged { get; set; }
    public string? ApiKey { get; set; }
    public override string ToString()
        => $"{{ {nameof(LastChanged)}: {LastChanged}, {nameof(ApiKey)} : {ApiKey} }}";
}
