# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

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
