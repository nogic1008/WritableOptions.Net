# Benchmark

Compare this project with [Awesome.Net.WritableOptions](https://www.nuget.org/packages/Awesome.Net.WritableOptions)

## How To

### Environments

To run locally, you need to install:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)

You can also run this benchmark on [GitHub Actions](https://github.com/nogic1008/WritableOptions.Net/actions/workflows/benchmark.yml).

### Command

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

```
BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2340) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.203
  [Host]     : .NET 6.0.28 (6.0.2824.12007), X64 RyuJIT AVX2
  Job-BNYRWD : .NET 6.0.28 (6.0.2824.12007), X64 RyuJIT AVX2
  Job-CXRSMJ : .NET 7.0.17 (7.0.1724.11508), X64 RyuJIT AVX2
  Job-UHTKGC : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-REYCYN : .NET Framework 4.8.1 (4.8.9186.0), X64 RyuJIT VectorSize=256

InvocationCount=1  UnrollFactor=1  
```

| Method                       | Runtime            | Mean     | Error   | StdDev   | Median   | Ratio | RatioSD | Gen0       | Gen1      | Allocated | Alloc Ratio |
|----------------------------- |------------------- |---------:|--------:|---------:|---------:|------:|--------:|-----------:|----------:|----------:|------------:|
| Awesome.Net.WritableOptions  | .NET 6.0           | 271.6 ms | 1.57 ms |  1.22 ms | 271.5 ms |  1.30 |    0.01 |  3000.0000 |         - |  63.43 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 6.0           | 208.8 ms | 0.87 ms |  0.73 ms | 209.0 ms |  1.00 |    0.00 |  2000.0000 |         - |  32.08 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 6.0           | 175.5 ms | 0.75 ms |  0.63 ms | 175.3 ms |  0.84 |    0.00 |  2000.0000 | 1000.0000 |  32.85 MB |        1.02 |
|                              |                    |          |         |          |          |       |         |            |           |           |             |
| Awesome.Net.WritableOptions  | .NET 7.0           | 268.2 ms | 3.57 ms |  2.98 ms | 266.9 ms |  1.26 |    0.02 |  3000.0000 |         - |  63.26 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 7.0           | 212.1 ms | 4.16 ms |  3.69 ms | 210.5 ms |  1.00 |    0.00 |  2000.0000 |         - |  31.99 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 7.0           | 174.2 ms | 1.80 ms |  1.50 ms | 173.7 ms |  0.82 |    0.02 |  2000.0000 |         - |  32.85 MB |        1.03 |
|                              |                    |          |         |          |          |       |         |            |           |           |             |
| Awesome.Net.WritableOptions  | .NET 8.0           | 243.5 ms | 2.14 ms |  1.67 ms | 243.2 ms |  1.26 |    0.01 |  3000.0000 |         - |  63.25 MB |        1.98 |
| (NuGet)Nogic.WritableOptions | .NET 8.0           | 192.9 ms | 1.58 ms |  1.32 ms | 192.7 ms |  1.00 |    0.00 |  2000.0000 |         - |  31.99 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET 8.0           | 162.2 ms | 2.45 ms |  2.05 ms | 161.9 ms |  0.84 |    0.01 |  2000.0000 |         - |  32.86 MB |        1.03 |
|                              |                    |          |         |          |          |       |         |            |           |           |             |
| Awesome.Net.WritableOptions  | .NET Framework 4.8 | 437.8 ms | 8.48 ms |  9.76 ms | 434.2 ms |  1.32 |    0.05 | 10000.0000 |         - |   64.4 MB |        2.00 |
| (NuGet)Nogic.WritableOptions | .NET Framework 4.8 | 334.0 ms | 6.39 ms | 12.31 ms | 326.9 ms |  1.00 |    0.00 |  5000.0000 |         - |  32.15 MB |        1.00 |
| (Dev)Nogic.WritableOptions   | .NET Framework 4.8 | 290.9 ms | 5.11 ms |  4.53 ms | 288.9 ms |  0.88 |    0.03 |  5000.0000 | 1000.0000 |  32.92 MB |        1.02 |

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
