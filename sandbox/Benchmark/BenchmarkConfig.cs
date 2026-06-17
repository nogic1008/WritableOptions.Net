using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig() =>
        AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core90))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithRuntime(ClrRuntime.Net48))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddDiagnoser(MemoryDiagnoser.Default);
}
