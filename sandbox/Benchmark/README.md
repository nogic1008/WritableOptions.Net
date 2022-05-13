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
for (int i = 0; i < 100; i++)
{
    _awesomeWritableOptions.Update(o =>
    {
        o.LastLaunchedAt = _option.LastLaunchedAt;
        o.StringSettings = _option.StringSettings;
        o.IntSettings = _option.IntSettings;
    });
}

// Nogic.WritableOptions<T>
for (int i = 0; i < 100; i++)
{
    _myWritableOptions.Update(_option);
}
```

## Result

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.20348
Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.202
  [Host]     : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  Job-DTZKXX : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  Job-DXZGAX : .NET Core 3.1.24 (CoreCLR 4.700.22.16002, CoreFX 4.700.22.17909), X64 RyuJIT
  Job-GZJUJV : .NET Framework 4.8 (4.8.4470.0), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  
```

|                        Method |        Job |            Runtime |     Mean |   Error |  StdDev | Ratio |
|------------------------------ |----------- |------------------- |---------:|--------:|--------:|------:|
| AwesomeWritableOptions_Update | Job-DTZKXX |           .NET 6.0 | 404.3 ms | 6.94 ms | 5.79 ms |  1.00 |
|      MyWritableOptions_Update | Job-DTZKXX |           .NET 6.0 | 264.5 ms | 2.72 ms | 2.41 ms |  0.65 |
|                               |            |                    |          |         |         |       |
| AwesomeWritableOptions_Update | Job-DXZGAX |      .NET Core 3.1 | 444.5 ms | 6.88 ms | 6.10 ms |  1.00 |
|      MyWritableOptions_Update | Job-DXZGAX |      .NET Core 3.1 | 295.0 ms | 5.30 ms | 4.70 ms |  0.66 |
|                               |            |                    |          |         |         |       |
| AwesomeWritableOptions_Update | Job-GZJUJV | .NET Framework 4.8 | 640.3 ms | 7.18 ms | 6.72 ms |  1.00 |
|      MyWritableOptions_Update | Job-GZJUJV | .NET Framework 4.8 | 421.7 ms | 6.05 ms | 5.06 ms |  0.66 |
