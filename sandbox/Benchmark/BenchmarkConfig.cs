using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.ShortRun)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }
}
