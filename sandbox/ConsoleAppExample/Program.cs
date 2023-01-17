using Microsoft.Extensions.Logging;
using Nogic.WritableOptions;

const string ApplicationName = "ConsoleAppExample";

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices(static (ctx, services) =>
{
    // Load app settings
    var config = ctx.Configuration;
    string appName = ctx.HostingEnvironment.ApplicationName ?? ApplicationName;
    services.ConfigureWritable<AppOption>(config.GetSection(appName));
});

var rootEventId = new EventId(0, $"{ApplicationName}.Root");
var logInfomation = LoggerMessage.Define<string>(LogLevel.Information, rootEventId, "{Message}");
var logDebug = LoggerMessage.Define<string>(LogLevel.Debug, rootEventId, "{Message}");

var app = builder.Build();
app.AddRootCommand((ConsoleAppContext ctx, IWritableOptions<AppOption> writableOptions) =>
{
    logDebug(ctx.Logger, "Start.", null);

    var currentOption = writableOptions.Value;
    AppOption.LogInfomation(ctx.Logger, currentOption);

    var newOption = new AppOption { LastChanged = ctx.Timestamp, ApiKey = Guid.NewGuid().ToString() };
    AppOption.LogInfomation(ctx.Logger, newOption);

    logInfomation(ctx.Logger, "Try to write new settings.", null);
    writableOptions.Update(newOption, true);
    logInfomation(ctx.Logger, "Success! Check your appsettings.json.", null);
    AppOption.LogInfomation(ctx.Logger, writableOptions.Value);

    logDebug(ctx.Logger, "End.", null);
});

app.Run();
