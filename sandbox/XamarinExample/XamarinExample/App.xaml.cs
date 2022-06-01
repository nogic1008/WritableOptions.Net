using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nogic.WritableOptions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinExample;

public partial class App : Application
{
    private const string SettingsFileName = "appsettings.json";

    internal static void CreateSettingFile(string path, string defaultContent = "{}")
    {
        if (!File.Exists(path))
            File.WriteAllText(path, defaultContent, Encoding.UTF8);
    }

    private readonly IServiceProvider _services;
    private readonly IConfigurationRoot _configuration;

    public App()
    {
        InitializeComponent();
        CreateSettingFile(Path.Combine(FileSystem.AppDataDirectory, SettingsFileName));

        // Configuration
        _configuration = new ConfigurationBuilder()
            .AddJsonFile(new PhysicalFileProvider(FileSystem.AppDataDirectory), SettingsFileName, false, true)
            .Build();

        // DI
        _services = new ServiceCollection()
            .AddSingleton<IConfigurationRoot>(_configuration)
            .ConfigureWritableWithExplicitPath<AppOption>(_configuration.GetSection(nameof(AppOption)), FileSystem.AppDataDirectory, SettingsFileName)
            .AddTransient<MainPage>()
            .AddSingleton<App>(this)
            .BuildServiceProvider();

        MainPage = _services.GetRequiredService<MainPage>();
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }
}
