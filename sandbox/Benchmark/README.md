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
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.20348.1006), VM=Hyper-V
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=7.0.100-rc.1.22431.12
  [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  Job-OLWNIV : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  Job-BHOJCA : .NET 7.0.0 (7.0.22.42610), X64 RyuJIT AVX2
  Job-NSGOZA : .NET Core 3.1.29 (CoreCLR 4.700.22.41602, CoreFX 4.700.22.41702), X64 RyuJIT AVX2
  Job-MUMWMC : .NET Framework 4.8.1 (4.8.9093.0), X64 RyuJIT VectorSize=256

InvocationCount=1  UnrollFactor=1  
```

|                       Method |            Runtime |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |       Gen0 |      Gen1 | Allocated | Alloc Ratio |
|----------------------------- |------------------- |---------:|---------:|---------:|---------:|------:|--------:|-----------:|----------:|----------:|------------:|
|  Awesome.Net.WritableOptions |           .NET 6.0 | 330.1 ms |  1.31 ms |  1.10 ms | 330.1 ms |  1.54 |    0.01 |  3000.0000 |         - |  63.43 MB |        1.98 |
|        Nogic.WritableOptions |           .NET 6.0 | 214.2 ms |  1.49 ms |  1.16 ms | 214.1 ms |  1.00 |    0.01 |  1000.0000 |         - |  32.11 MB |        1.00 |
|                              |                    |          |          |          |          |       |         |            |           |           |             |
|  Awesome.Net.WritableOptions |           .NET 7.0 | 327.7 ms |  6.55 ms | 15.57 ms | 318.4 ms |  1.60 |    0.08 |  3000.0000 |         - |  63.26 MB |        1.98 |
|        Nogic.WritableOptions |           .NET 7.0 | 206.0 ms |  3.78 ms |  3.54 ms | 205.1 ms |  1.01 |    0.02 |  1000.0000 |         - |  32.02 MB |        1.00 |
|                              |                    |          |          |          |          |       |         |            |           |           |             |
|  Awesome.Net.WritableOptions |      .NET Core 3.1 | 363.2 ms |  6.92 ms |  5.40 ms | 361.3 ms |  1.53 |    0.02 |  3000.0000 |         - |  63.53 MB |        1.97 |
|        Nogic.WritableOptions |      .NET Core 3.1 | 239.3 ms |  1.13 ms |  0.94 ms | 239.4 ms |  1.01 |    0.00 |  1000.0000 |         - |  32.24 MB |        1.00 |
|                              |                    |          |          |          |          |       |         |            |           |           |             |
|  Awesome.Net.WritableOptions | .NET Framework 4.8 | 528.3 ms | 10.20 ms | 14.63 ms | 518.6 ms |  1.52 |    0.10 | 10000.0000 | 1000.0000 |  64.52 MB |        1.99 |
|        Nogic.WritableOptions | .NET Framework 4.8 | 338.8 ms |  5.91 ms | 11.40 ms | 333.4 ms |  0.98 |    0.05 |  5000.0000 |         - |  32.35 MB |        1.00 |
