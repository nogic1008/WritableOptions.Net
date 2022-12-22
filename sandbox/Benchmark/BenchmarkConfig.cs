using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
        => AddJob(Job.Default.WithRuntime(CoreRuntime.Core70))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithRuntime(ClrRuntime.Net48))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddDiagnoser(MemoryDiagnoser.Default);
}
