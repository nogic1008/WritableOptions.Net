namespace ConsoleAppExample;

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
        const string MethodName = $"{nameof(AppBase)}.{nameof(Run)}()";
        Context.Logger.LogDebug("{MethodName} Start.", MethodName);

        var currentOption = _writableOptions.Value;
        Context.Logger.LogInformation("Current Settings: {currentOption}", currentOption);

        var newOption = new AppOption { LastLaunchedAt = Context.Timestamp, ApiKey = Guid.NewGuid().ToString() };
        Context.Logger.LogInformation("New Settings: {newOption}", newOption);

        Context.Logger.LogInformation("Try to write new settings.");
        _writableOptions.Update(newOption);
        Context.Logger.LogInformation("Success! Check your appsettings.json.");

        Context.Logger.LogDebug("{MethodName} End.", MethodName);
    }
}
