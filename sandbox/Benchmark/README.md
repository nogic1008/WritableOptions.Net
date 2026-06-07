# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

## How To

### Environments

To run locally, you need to install:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)

You can also run this benchmark on [GitHub Actions](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/benchmark.yml).

### Command

```console
> dotnet run -c Release -f net10.0
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

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32860/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Job-ILQBPD : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  Job-QQBMPU : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3
  Job-JRXVGN : .NET 9.0.16 (9.0.16, 9.0.1626.22923), X64 RyuJIT x86-64-v3
  Job-HHXQJW : .NET Framework 4.8.1 (4.8.9325.0), X64 RyuJIT VectorSize=256

InvocationCount=1  UnrollFactor=1
```

| Method                       | Runtime            | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0       | Allocated | Alloc Ratio |
|----------------------------- |------------------- |---------:|---------:|---------:|---------:|------:|--------:|-----------:|----------:|------------:|
| Awesome.Net.WritableOptions  | .NET 10.0          | 316.6 ms |  6.11 ms | 16.40 ms | 308.7 ms |  1.29 |    0.07 |  3000.0000 |   63.3 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 10.0          | 245.9 ms |  4.39 ms |  3.66 ms | 245.2 ms |  1.00 |    0.02 |  2000.0000 |  32.01 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 10.0          | 202.7 ms |  3.82 ms |  3.39 ms | 201.9 ms |  0.82 |    0.02 |  2000.0000 |  32.87 MB |        1.03 |
|                              |                    |          |          |          |          |       |         |            |           |             |
| Awesome.Net.WritableOptions  | .NET 8.0           | 323.9 ms |  6.42 ms | 11.74 ms | 319.8 ms |  1.27 |    0.05 |  3000.0000 |   63.3 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 8.0           | 255.2 ms |  3.85 ms |  5.64 ms | 253.1 ms |  1.00 |    0.03 |  2000.0000 |  32.01 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 8.0           | 205.2 ms |  1.53 ms |  1.50 ms | 205.1 ms |  0.80 |    0.02 |  2000.0000 |  32.87 MB |        1.03 |
|                              |                    |          |          |          |          |       |         |            |           |             |
| Awesome.Net.WritableOptions  | .NET 9.0           | 309.4 ms |  4.07 ms |  3.18 ms | 309.7 ms |  1.23 |    0.06 |  3000.0000 |   63.3 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 9.0           | 251.9 ms |  5.02 ms | 13.04 ms | 245.4 ms |  1.00 |    0.07 |  2000.0000 |  32.01 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 9.0           | 222.1 ms |  8.82 ms | 24.58 ms | 211.0 ms |  0.88 |    0.11 |  2000.0000 |  32.87 MB |        1.03 |
|                              |                    |          |          |          |          |       |         |            |           |             |
| Awesome.Net.WritableOptions  | .NET Framework 4.8 | 524.8 ms | 10.48 ms | 17.80 ms | 517.0 ms |  1.28 |    0.10 | 10000.0000 |  64.44 MB |        2.00 |
| (NuGet)Nogic.WritableOptions | .NET Framework 4.8 | 413.9 ms | 11.65 ms | 32.67 ms | 399.4 ms |  1.01 |    0.11 |  5000.0000 |  32.15 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET Framework 4.8 | 351.1 ms |  6.41 ms |  5.00 ms | 349.7 ms |  0.85 |    0.06 |  5000.0000 |  32.93 MB |        1.02 |

- Mean: Arithmetic mean of all measurements
- Error: Half of 99.9% confidence interval
- StdDev: Standard deviation of all measurements
- Median: Value separating the higher half of all measurements (50th percentile)
- Ratio: Mean of the ratio distribution ([Current]/[Baseline])
- RatioSD: Standard deviation of the ratio distribution ([Current]/[Baseline])
- Gen 0: GC Generation 0 collects per 1000 operations
- Allocated: Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
- Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
- 1 ms: 1 Millisecond (0.001 sec)
