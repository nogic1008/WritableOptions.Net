using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nogic.WritableOptions;

namespace MauiExampleNet10;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // MAUI
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuration
        builder.Configuration.AddJsonFile("appsettings.json", true, true);

        // DI
        builder.Services
            .ConfigureWritable<AppOption>(builder.Configuration.GetSection(nameof(AppOption)))
            .AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
