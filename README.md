# WritableOptions.Net

Provides `Options<T>` that can update values to source JSON file

[![NuGet](https://img.shields.io/nuget/v/Nogic.WritableOptions?label=NuGet&logo=nuget&logoColor=blue)](https://www.nuget.org/packages/Nogic.WritableOptions/)
[![GitHub release](https://img.shields.io/github/v/release/nogic1008/WritableOptions.Net?include_prereleases&logo=github&sort=semver)](https://github.com/nogic1008/WritableOptions.Net/releases)
[![.NET CI](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/nogic1008/WritableOptions.Net/branch/main/graph/badge.svg?token=SjTS03boND)](https://codecov.io/gh/nogic1008/WritableOptions.Net)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net/badge)](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net)
[![License](https://img.shields.io/github/license/nogic1008/WritableOptions.Net)](LICENSE)

This is a fork of [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions), but:

- Supports [UTF-8+BOM](https://github.com/nogic1008/WritableOptions.Net/issues/55) files
- improves performance by using `Span<T>` (see [benchmark](https://github.com/nogic1008/WritableOptions.Net/tree/main/sandbox/Benchmark))

See also: [How to update values into appsetting.json?](https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json)

## Usage

See [ConsoleApp Example](https://github.com/nogic1008/WritableOptions.Net/tree/main/sandbox/ConsoleAppExample/).

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
