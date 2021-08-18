namespace ConsoleAppExample;

using System;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using Nogic.WritableOptions;

public class AppBase : ConsoleAppBase
{
    private readonly IWritableOptions<AppOption> _writableOptions;
    public AppBase(IWritableOptions<AppOption> writableOptions)
        => _writableOptions = writableOptions ?? throw new ArgumentNullException(nameof(writableOptions));

    public void Run()
    {
        Context.Logger.LogDebug($"{nameof(AppBase)}.{nameof(Run)}() Start.");

        Context.Logger.LogInformation("Current Settings: {0}", _writableOptions.Value);

        var newOption = new AppOption { LastLaunchedAt = Context.Timestamp, ApiKey = Guid.NewGuid().ToString() };
        Context.Logger.LogInformation("New Settings: {0}", newOption);

        Context.Logger.LogInformation("Try to write new settings.");
        _writableOptions.Update(newOption);
        Context.Logger.LogInformation("Success! Check your appsettings.json.");

        Context.Logger.LogDebug($"{nameof(AppBase)}.{nameof(Run)}() End.");
    }
}
