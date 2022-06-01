using ConsoleAppExample;
using Microsoft.Extensions.Logging;
using Nogic.WritableOptions;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
    services.ConfigureWritable<AppOption>(ctx.Configuration.GetSection(ctx.HostingEnvironment.ApplicationName)));

var rootEventId = new EventId(0, $"{nameof(ConsoleAppExample)}.Root");
var logInfomation = LoggerMessage.Define<string>(LogLevel.Information, rootEventId, "{Message}");
var logDebug = LoggerMessage.Define<string>(LogLevel.Debug, rootEventId, "{Message}");

var app = builder.Build();
app.AddRootCommand((ConsoleAppContext ctx, IWritableOptions<AppOption> writableOptions) =>
{
    logDebug(ctx.Logger, "Start.", null);

    var currentOption = writableOptions.Value;
    AppOption.LogInfomation(ctx.Logger, currentOption);

    var newOption = new AppOption { LastLaunchedAt = ctx.Timestamp, ApiKey = Guid.NewGuid().ToString() };
    AppOption.LogInfomation(ctx.Logger, newOption);

    logInfomation(ctx.Logger, "Try to write new settings.", null);
    writableOptions.Update(newOption);
    logInfomation(ctx.Logger, "Success! Check your appsettings.json.", null);

    logDebug(ctx.Logger, "End.", null);
});

app.Run();
