global using Nogic.WritableOptions;
using Microsoft.Extensions.Configuration;

namespace MauiExample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        builder.Configuration.AddJsonFile("appsettings.json", true, true);
        builder.Services.ConfigureWritable<AppOption>(builder.Configuration.GetSection(nameof(AppOption)));
        builder.Services.AddSingleton<MainPage>();
        return builder.Build();
    }
}
