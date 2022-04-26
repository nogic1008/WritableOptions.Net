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
    private static string settingsFileName = "appsettings.json";

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
        CreateSettingFile(Path.Combine(FileSystem.AppDataDirectory, settingsFileName));
        var sc = new ServiceCollection();
        var config = new ConfigurationBuilder();
        config.AddJsonFile(new PhysicalFileProvider(FileSystem.AppDataDirectory), settingsFileName, false, true);
        _configuration = config.Build();
        sc.AddSingleton<IConfigurationRoot>(_configuration);
        sc.ConfigureWritableWithExplicitPath<AppOption>(_configuration.GetSection(nameof(AppOption)),
            FileSystem.AppDataDirectory, settingsFileName);
        sc.AddTransient<MainPage>();
        sc.AddSingleton<App>(this);
        _services = sc.BuildServiceProvider();
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
