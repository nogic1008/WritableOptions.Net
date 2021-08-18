# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

## How To

```console
> dotnet run -c Release --filter *WritableOptionsBenchmark*
```

## Test Code

[View all](./WritableOptionsBenchmark.cs)

```cs
// Awesome.Net.WritableOptions<T>
for (int i = 0; i < 1000; i++)
{
    _awesomeWritableOptions.Update(o =>
    {
        o.LastLaunchedAt = _option.LastLaunchedAt;
        o.StringSettings = _option.StringSettings;
        o.IntSettings = _option.IntSettings;
    });
}

// Nogic.WritableOptions<T>
for (int i = 0; i < 1000; i++)
{
    _myWritableOptions.Update(_option);
}
```

## Result

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1139 (1909/November2019Update/19H2)
AMD Ryzen 5 2500U with Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.302
  [Host]     : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT
  Job-GUADFV : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  Job-VCYLKU : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |       Runtime |    Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------------ |----------- |-------------- |--------:|---------:|---------:|------:|--------:|
| AwesomeWritableOptions_Update | Job-GUADFV |      .NET 5.0 | 4.530 s | 0.0873 s | 0.0971 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-GUADFV |      .NET 5.0 | 3.756 s | 0.0513 s | 0.0480 s |  0.83 |    0.02 |
|                               |            |               |         |          |          |       |         |
| AwesomeWritableOptions_Update | Job-VCYLKU | .NET Core 3.1 | 4.644 s | 0.0862 s | 0.0764 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-VCYLKU | .NET Core 3.1 | 3.930 s | 0.0781 s | 0.0836 s |  0.85 |    0.02 |
