global using Nogic.WritableOptions;
using Microsoft.Extensions.Configuration;

namespace MauiExample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // MAUI
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        // Configuration
        builder.Configuration.AddJsonFile("appsettings.json", true, true);

        // DI
        builder.Services
            .ConfigureWritable<AppOption>(builder.Configuration.GetSection(nameof(AppOption)))
            .AddSingleton<MainPage>();

        return builder.Build();
    }
}
