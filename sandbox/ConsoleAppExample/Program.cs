using ConsoleAppExample;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Nogic.WritableOptions;

await Host.CreateDefaultBuilder()
    .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
    .ConfigureServices((context, services) =>
    {
        // Load app settings
        var config = context.Configuration;
        services.ConfigureWritable<AppOption>(config.GetSection(context.HostingEnvironment.ApplicationName));
    })
    .RunConsoleAppFrameworkAsync<AppBase>(args)
    .ConfigureAwait(false);
