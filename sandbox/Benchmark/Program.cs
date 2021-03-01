using Benchmark;
using BenchmarkDotNet.Running;

BenchmarkSwitcher
    .FromAssemblies(new[] { typeof(WritableOptionsBenchmark).Assembly })
    .Run(args);
