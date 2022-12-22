# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

## How To

```console
> dotnet run -c Release -f net6.0
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
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.20348.1366), VM=Hyper-V
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2
  Job-GPASGY : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2
  Job-GPMNYX : .NET 7.0.1 (7.0.122.56804), X64 RyuJIT AVX2
  Job-EEPWZO : .NET Framework 4.8.1 (4.8.9105.0), X64 RyuJIT VectorSize=256

InvocationCount=1  UnrollFactor=1  
```

|                      Method |            Runtime |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |       Gen0 |      Gen1 | Allocated | Alloc Ratio |
|---------------------------- |------------------- |---------:|---------:|---------:|---------:|------:|--------:|-----------:|----------:|----------:|------------:|
| Awesome.Net.WritableOptions |           .NET 6.0 | 331.8 ms |  2.12 ms |  1.66 ms | 331.5 ms |  1.34 |    0.01 |  3000.0000 |         - |  63.43 MB |        1.98 |
|       Nogic.WritableOptions |           .NET 6.0 | 247.8 ms |  1.04 ms |  0.87 ms | 248.0 ms |  1.00 |    0.00 |  1000.0000 |         - |  32.11 MB |        1.00 |
|                             |                    |          |          |          |          |       |         |            |           |           |             |
| Awesome.Net.WritableOptions |           .NET 7.0 | 322.5 ms |  6.45 ms | 13.32 ms | 316.0 ms |  1.37 |    0.06 |  3000.0000 |         - |  63.26 MB |        1.98 |
|       Nogic.WritableOptions |           .NET 7.0 | 238.0 ms |  1.11 ms |  0.93 ms | 238.0 ms |  1.00 |    0.00 |  1000.0000 |         - |  32.02 MB |        1.00 |
|                             |                    |          |          |          |          |       |         |            |           |           |             |
| Awesome.Net.WritableOptions | .NET Framework 4.8 | 531.8 ms | 10.39 ms | 17.08 ms | 521.3 ms |  1.33 |    0.07 | 10000.0000 | 1000.0000 |  64.52 MB |        1.99 |
|       Nogic.WritableOptions | .NET Framework 4.8 | 401.3 ms |  8.00 ms | 18.07 ms | 390.6 ms |  1.00 |    0.00 |  5000.0000 |         - |  32.35 MB |        1.00 |

- Mean        : Arithmetic mean of all measurements
- Error       : Half of 99.9% confidence interval
- StdDev      : Standard deviation of all measurements
- Median      : Value separating the higher half of all measurements (50th percentile)
- Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
- RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
- Gen0        : GC Generation 0 collects per 1000 operations
- Gen1        : GC Generation 1 collects per 1000 operations
- Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
- Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
- 1 ms        : 1 Millisecond (0.001 sec)
