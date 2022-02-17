using ConsoleAppExample;
using Microsoft.Extensions.Logging;
using Nogic.WritableOptions;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    // Load app settings
    var config = ctx.Configuration;
    services.ConfigureWritable<AppOption>(config.GetSection(ctx.HostingEnvironment.ApplicationName));
});

var app = builder.Build();
app.AddRootCommand((ConsoleAppContext ctx, IWritableOptions<AppOption> writableOptions) =>
{
    ctx.Logger.LogDebug("Start.");

    var currentOption = writableOptions.Value;
    ctx.Logger.LogInformation("Current Settings: {CurrentOption}", currentOption);

    var newOption = new AppOption { LastLaunchedAt = ctx.Timestamp, ApiKey = Guid.NewGuid().ToString() };
    ctx.Logger.LogInformation("New Settings: {NewOption}", newOption);

    ctx.Logger.LogInformation("Try to write new settings.");
    writableOptions.Update(newOption);
    ctx.Logger.LogInformation("Success! Check your appsettings.json.");

    ctx.Logger.LogDebug("End.");
});

app.Run();
