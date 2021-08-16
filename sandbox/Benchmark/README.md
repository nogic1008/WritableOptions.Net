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
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.18363.1139 (1909/November2019Update/19H2)
AMD Ryzen 5 2500U with Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.302
  [Host]     : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT
  Job-PDQTZU : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  Job-OAHQJI : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |       Runtime |    Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------------ |----------- |-------------- |--------:|---------:|---------:|------:|--------:|
| AwesomeWritableOptions_Update | Job-PDQTZU |      .NET 5.0 | 4.743 s | 0.0948 s | 0.1419 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-PDQTZU |      .NET 5.0 | 3.916 s | 0.0766 s | 0.0679 s |  0.83 |    0.02 |
|                               |            |               |         |          |          |       |         |
| AwesomeWritableOptions_Update | Job-OAHQJI | .NET Core 3.1 | 4.647 s | 0.0575 s | 0.0538 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-OAHQJI | .NET Core 3.1 | 3.919 s | 0.0536 s | 0.0475 s |  0.84 |    0.01 |
