namespace MauiExample;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        InitializeComponent();
        _services = services;
        MainPage = _services.GetRequiredService<MainPage>();
    }
}
