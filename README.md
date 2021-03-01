# WritableOptions.Net

[![NuGet](https://img.shields.io/nuget/v/Nogic.WritableOptions?label=NuGet&logo=nuget&logoColor=blue)](https://www.nuget.org/packages/Nogic.WritableOptions/)
[![GitHub release](https://img.shields.io/github/v/release/nogic1008/WritableOptions.Net?include_prereleases&logo=github&sort=semver)](https://github.com/nogic1008/WritableOptions.Net/releases)
[![.NET Core CI](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnetcore.yml/badge.svg)](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/dotnetcore.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net/badge)](https://www.codefactor.io/repository/github/nogic1008/WritableOptions.Net)
[![License](https://img.shields.io/github/license/nogic1008/WritableOptions.Net)](LICENSE)

This is a fork of [Awesome.Net.WritableOptions](https://github.com/Nongzhsh/Awesome.Net.WritableOptions), but:

- Fixes [the issue](https://github.com/Nongzhsh/Awesome.Net.WritableOptions/issues/1) with `Microsoft.Extensions.Configuration.Json` >= 3.0.0
- improves performance by using `System.Text.Json`

See also: [How to update values into appsetting.json?](https://stackoverflow.com/questions/40970944/how-to-update-values-into-appsetting-json)

## Usage

See [ConsoleApp Example](./sandbox/ConsoleAppExample/).

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

## Benchmark

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.103
  [Host]     : .NET Core 3.1.12 (CoreCLR 4.700.21.6504, CoreFX 4.700.21.6905), X64 RyuJIT
  Job-VJLHPX : .NET Core 3.1.12 (CoreCLR 4.700.21.6504, CoreFX 4.700.21.6905), X64 RyuJIT
  Job-KHBNND : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |       Runtime |     Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------------ |-------------- |---------:|---------:|---------:|------:|--------:|
| AwesomeWritableOptions_Update | .NET Core 3.1 | 791.3 ms | 15.70 ms | 16.12 ms |  1.00 |    0.00 |
|      MyWritableOptions_Update | .NET Core 3.1 | 464.0 ms | 11.90 ms | 35.09 ms |  0.61 |    0.03 |
|                               |               |          |          |          |       |         |
| AwesomeWritableOptions_Update | .NET 5.0      | 754.1 ms | 15.07 ms | 27.17 ms |  1.00 |    0.00 |
|      MyWritableOptions_Update | .NET 5.0      | 404.5 ms |  8.47 ms | 24.85 ms |  0.55 |    0.04 |
