# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

## How To

```console
> dotnet run -c Release -f net6.0 --filter *WritableOptionsBenchmark*
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
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.928 (21H1/May2021Update)
AMD Ryzen 5 2500U with Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-AQXYGX : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-EIGSMZ : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |       Runtime |    Mean |    Error |   StdDev |  Median | Ratio | RatioSD |
|------------------------------ |----------- |-------------- |--------:|---------:|---------:|--------:|------:|--------:|
| AwesomeWritableOptions_Update | Job-AQXYGX |      .NET 6.0 | 2.922 s | 0.1199 s | 0.3535 s | 2.736 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-AQXYGX |      .NET 6.0 | 1.695 s | 0.0631 s | 0.1862 s | 1.758 s |  0.59 |    0.10 |
|                               |            |               |         |          |          |         |       |         |
| AwesomeWritableOptions_Update | Job-EIGSMZ | .NET Core 3.1 | 3.245 s | 0.1130 s | 0.3333 s | 3.302 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-EIGSMZ | .NET Core 3.1 | 1.743 s | 0.0648 s | 0.1910 s | 1.787 s |  0.54 |    0.08 |
