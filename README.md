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

This library is intended for use with [.NET Generic Host](https://learn.microsoft.com/aspnet/core/fundamentals/host/generic-host) context.

### Setup

```csharp
using Nogic.WritableOptions;

// Load config from { "MySection": {(here)} }
IConfigurationSection section = context.Configration.GetSection("MySection");

// Save & Load from appsettings.json (from root folder)
services.ConfigureWritable<MyOptions>(section);

// Save & Load from (Special Folder)/appsettings.json.
// It is useful for Xamarin
services.ConfigureWritableWithExplicitPath<AppOption>(section, FileSystem.AppDataDirectory);
```

### Consume

```csharp
public class AppBase
{
    private readonly IWritableOptions<MyOptions> _writableOptions;

    // Constructor DI
    public AppBase(IWritableOptions<MyOptions> writableOptions)
      => _writableOptions = writableOptions;

    public void Run()
    {
        // Read value via IOptions<T>
        var option = _writableOptions.Value;

        // Write value via IWritableOptions<T>
        var newOption = new MyOptions();
        _writableOptions.Update(newOption, reload: true);
    }
}
```

### Sample

- [Console App](https://github.com/nogic1008/WritableOptions.Net/tree/main/sandbox/ConsoleAppExample/)
- [.NET MAUI App](https://github.com/nogic1008/WritableOptions.Net/tree/main/sandbox/MauiExample/)
- [Blazor Server App](https://github.com/nogic1008/WritableOptions.Net/tree/main/sandbox/BlazorExample/)
