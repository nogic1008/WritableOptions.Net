using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nogic.WritableOptions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinExample;

public partial class App : Application
{
    private const string SettingsFileName = "appsettings.json";

    internal static void CreateSettingFile(string path, string defaultContent = "{}")
    {
        if (!File.Exists(path))
            File.WriteAllText(path, defaultContent, Encoding.UTF8);
    }

    readonly IServiceProvider _services;
    readonly IConfigurationRoot _configuration;

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
