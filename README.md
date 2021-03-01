# WritableOptions.Net

[![NuGet](https://img.shields.io/nuget/v/Nogic.WritableOptions?label=NuGet&logo=nuget&logoColor=blue)](https://www.nuget.org/packages/Nogic.WritableOptions/)
[![GitHub release](https://img.shields.io/github/v/release/nogic1008/WritableOptions.Net?include_prereleases&logo=github&sort=semver)](https://github.com/nogic1008/WritableOptions.Net/releases)
[![.NET Core CI](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnetcore.yml/badge.svg)](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnetcore.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net/badge)](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net)
[![License](https://img.shields.io/github/license/nogic1008/WritableOptions.Net)](LICENSE)

This is a fork of [Awesome.Net.WritableOptions](https://github.com/Nongzhsh/Awesome.Net.WritableOptions), but written for .NET Core 3.1 or greater.

See also: [How to update values into appsetting.json?](https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json)

## Usage

See also [ConsoleApp Example](./sandbox/ConsoleAppExample/)

```csharp
using Nogic.WritableOptions;

// ...
// in IHostBuilder.ConfigureServices((context, services) => {...})

// Save & Load from appsettings.json
services.ConfigureWritable<MyOptions>(context.Configration.GetSection("MySection"));
// Or use custom JSON file
services.ConfigureWritable<MyOptions>(context.Configration.GetSection("MySection"), "Resources/mysettings.json");
```

```csharp
public class AppBase
{
    private readonly IWritableOptions<MyOptions> _writableOptions;
    public AppBase(IWritableOptions<MyOptions> writableOptions) => _writableOptions = writableOptions;

    public void Run()
    {
        // IOptions<T>
        var option = _writableOptions.Value;

        // IOptionsMonitor<T>
        var fooOption = _writableOptions.Get("Foo");

        // IWritableOptions<T>
        var newOption = new MyOptions();
        _writableOptions.Update(newOption);
    }
}
```
