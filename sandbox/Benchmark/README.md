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
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1139 (1909/November2019Update/19H2)
AMD Ryzen 5 2500U with Radeon Vega Mobile Gfx, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.100-rc.1.21458.32
  [Host]     : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
  Job-IAUYRL : .NET 6.0.0 (6.0.21.45113), X64 RyuJIT
  Job-EAIPNX : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |       Runtime |    Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------------ |----------- |-------------- |--------:|---------:|---------:|------:|--------:|
| AwesomeWritableOptions_Update | Job-IAUYRL |      .NET 6.0 | 4.125 s | 0.0784 s | 0.0695 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-IAUYRL |      .NET 6.0 | 3.369 s | 0.0673 s | 0.1162 s |  0.82 |    0.04 |
|                               |            |               |         |          |          |       |         |
| AwesomeWritableOptions_Update | Job-EAIPNX | .NET Core 3.1 | 4.167 s | 0.0829 s | 0.1106 s |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-EAIPNX | .NET Core 3.1 | 3.497 s | 0.0687 s | 0.1221 s |  0.84 |    0.04 |
