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
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.103
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  Job-UIGFVB : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  Job-XUPPAH : .NET Core 3.1.23 (CoreCLR 4.700.22.11601, CoreFX 4.700.22.12208), X64 RyuJIT
  Job-QLMDRQ : .NET Framework 4.8 (4.8.4470.0), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |            Runtime |       Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------------ |----------- |------------------- |-----------:|---------:|---------:|------:|--------:|
| AwesomeWritableOptions_Update | Job-UIGFVB |           .NET 6.0 |   793.4 ms | 15.75 ms | 28.80 ms |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-UIGFVB |           .NET 6.0 |   470.2 ms |  9.19 ms | 11.95 ms |  0.59 |    0.03 |
|                               |            |                    |            |          |          |       |         |
| AwesomeWritableOptions_Update | Job-XUPPAH |      .NET Core 3.1 |   847.2 ms | 14.02 ms | 13.11 ms |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-XUPPAH |      .NET Core 3.1 |   512.9 ms |  6.48 ms |  5.75 ms |  0.61 |    0.01 |
|                               |            |                    |            |          |          |       |         |
| AwesomeWritableOptions_Update | Job-QLMDRQ | .NET Framework 4.8 | 1,316.9 ms | 25.98 ms | 40.45 ms |  1.00 |    0.00 |
|      MyWritableOptions_Update | Job-QLMDRQ | .NET Framework 4.8 | 1,105.1 ms | 20.42 ms | 19.10 ms |  0.83 |    0.03 |
