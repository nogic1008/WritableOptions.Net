namespace MauiExample;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services ?? throw new ArgumentNullException(nameof(services));
        MainPage = _services.GetRequiredService<MainPage>();
    }
}
