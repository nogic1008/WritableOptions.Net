using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddJob(Job.Default.WithRuntime(CoreRuntime.Core31))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}
